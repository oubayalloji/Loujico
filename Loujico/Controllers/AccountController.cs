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
        public AccountController(IConfiguration _configuration, UserManager<ApplicationUser> manager)
        {

            configuration = _configuration;
            userManager = manager;

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
                // تسجيل الخطأ
                /*   _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, new
                                   {
                                       Status = 500,
                                       Message = "An error occurred while processing your request",
                                       DetailedError = _env.IsDevelopment() ? ex.Message : null
                                   });*/
                return Ok();
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