using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Loujico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        Ilog ClsLogs;
        public AccountController(IConfiguration _configuration, UserManager<ApplicationUser> manager, Ilog clsLogs)
        {

            configuration = _configuration;
            userManager = manager;
            ClsLogs = clsLogs;
        }
        [HttpPost("LogIn")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<string>>> LogIn([FromForm] LogInModel model)
         {
            try
            {
                ApiResponse<string> response = new ApiResponse<string>();

                if (!ModelState.IsValid)//
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = "Invalid input data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var user = await userManager.FindByEmailAsync(model.email);
                if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                {

                    return NotFound(new ApiResponse<String>
                    {
                        Data = "Not Found",
                        Message = "خطأ في البريد الإلكتروني أو كلمة المرور",


                    });
                }
                if (user.IsDeleted == true)
                {
                    return NotFound(new ApiResponse<String>
                    {
                        Data = "Not Found",
                        Message = "المستخدم محذوف",


                    });
                }
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    user.LastVisit = DateTime.Now;
                    await userManager.UpdateAsync(user);
                    await ClsLogs.Add(user.Id, "LogIn", $"{user.UserName} has logged in");
                    return Ok(new ApiResponse<String>
                    {
                        Data = await GenerateToken(user),
                        Message = "Welcome Admin",
                        Role = "Admin"

                    });
                }
                else
                {
                    user.LastVisit = DateTime.Now;
                    await userManager.UpdateAsync(user);
                    return Ok(new ApiResponse<String>
                    {
                        Data = await GenerateToken(user),
                        Message = $"Welcome {user.UserName} ",
                        Role = "User"

                    });

                }
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<object>
                {
                    Message = ex.Message,

                });
            }

        }
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm] Register model)
        {
            // التحقق من صحة النموذج
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid input data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            // التحقق من وجود المستخدم مسبقاً
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return Conflict(new
                {
                    Status = 409,
                    Message = "The email address is already registered"
                });
            }

            // إنشاء مستخدم جديد
            var user = new ApplicationUser
            {
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                Email = model.Email,
                UserName = $"{model.UserName.Replace(" ", "_")}_{Guid.NewGuid().ToString()[..8]}",


            };

            try
            {
                // إنشاء الحساب
                var result = await userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = "User creation failed",
                        Errors = result.Errors.Select(e => e.Description)
                    });
                }

                // رفع صورة الملف الشخصي إذا وجدت
                /*   if (File1 != null && File1.Count > 0)
                   {
                       var uploadResult = await _imageService.UploadImageAsync(File1[0], "ProfileImg");
                       if (uploadResult.Success)
                       {
                           user. = uploadResult.FilePath;
                           await userManager.UpdateAsync(user);
                       }
                   }*/

                // تعيين دور "Seller" للمستخدم
                await userManager.AddToRoleAsync(user, model.Role);

                // إنشاء وتوقيع Token
                var token = await GenerateToken(user);

                return CreatedAtAction(nameof(Register), new
                {
                    Status = 201,
                    Data = new
                    {
                        Token = token,
                        UserId = user.Id,
                        IsAdmin = false // يمكنك التحقق من الأدوار إذا لزم الأمر
                    },
                    Message = "User registered successfully"
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<object>
                {
                    Message = ex.Message,

                });
            }
        }
        private async Task<string> GenerateToken(ApplicationUser user)
        {
            // الحصول على أدوار المستخدم (مثل "Admin")
            var userRoles = await userManager.GetRolesAsync(user); // تحتاج لجعل الدالة async

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.UserName)
    };

            // إضافة أدوار المستخدم كـ Claims
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // ClaimTypes.Role مهم للصلاحيات
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(48),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}