using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Loujico.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController : Controller
    {
        CompanySystemContext CTX;
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        Ilog ClsLogs;
        public AccountController(IConfiguration _configuration, UserManager<ApplicationUser> manager, Ilog clsLogs,CompanySystemContext systemContext)
        {
            CTX = systemContext;
            configuration = _configuration;
            userManager = manager;
            ClsLogs = clsLogs;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Team_Leader,Programmer")]
        [HttpGet("Header")]
        public async Task<ActionResult> Header()
        {
            try
            {
                var user = await userManager.GetUserAsync(User);
                var userName =  userManager.GetUserName(User);
                var roles = await userManager.GetRolesAsync(user);
                return Ok(new
                {
                    username = userName,
                    role = roles
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
           
                    user.LastVisit = DateTime.Now;
                    await userManager.UpdateAsync(user);
                    return Ok(new ApiResponse<String>
                    {
                        Data = await GenerateToken(user),
                        Message = $"Welcome {user.UserName} ",
  

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


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("UserList")]
        public async Task<ActionResult<ApiResponse<List<VmUserRoles>>>> UserList()
        {
            try 
            {
                var users = userManager.Users.ToList();
                var userRolesViewModel = new List<VmUserRoles>();

                foreach (var user in users)
                {
                    if (user.IsDeleted==true)
                    {
                        continue;
                    }
                    var roles = await userManager.GetRolesAsync(user);
                    userRolesViewModel.Add(new VmUserRoles
                    {
                        userid = user.Id,
                        username = user.UserName,
                        roles = roles,
                        Email = user.Email

                    });
                }

                return Ok(new ApiResponse<List<VmUserRoles>> { Data = userRolesViewModel });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<VmUserRoles>>
                {
                    Message = ex.Message,

                });

            }
        }
        [HttpGet("UserListId")]
        public async Task<ActionResult> UserListId()
        {
            try
            {
                var users = userManager.Users.ToList();
                var userRolesViewModel = new List<object>();

                foreach (var user in users)
                {
                    if (user.IsDeleted == true)
                    {
                        continue;
                    }
                    userRolesViewModel.Add(new 
                    {
                        userid = user.Id,
                        username =user.UserName

                    });
                }

                return Ok(new ApiResponse<List<object>> { Data = userRolesViewModel });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<VmUserRoles>>
                {
                    Message = ex.Message,

                });

            }
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("Add")]
        public async Task<IActionResult> Register([FromForm] Register model)
        {
            try
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
                if (existingUser == null)
                {
                    // إنشاء مستخدم جديد
                    var user = new ApplicationUser
                    {
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        Email = model.Email,
                        UserName = $"{model.UserName.Replace(" ", "_")}_{Guid.NewGuid().ToString()[..2]}",


                    };

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


                    // تعيين دور "Seller" للمستخدم
                    await userManager.AddToRoleAsync(user, model.roles);

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
           else if (existingUser.IsDeleted)
            {
                existingUser.UserName = model.UserName;
                existingUser.Email = model.Email;
                var currentRoles = await userManager.GetRolesAsync(existingUser);
                var selectedRole = model.roles;

                foreach (var role in currentRoles)
                {
                    await userManager.RemoveFromRoleAsync(existingUser, role);
                }

                if (!string.IsNullOrEmpty(selectedRole))
                {
                    await userManager.AddToRoleAsync(existingUser, selectedRole);
                }
                existingUser.IsDeleted= false;
                var result = await userManager.UpdateAsync(existingUser);
                return Ok("User has been restored");
            }
            if (!existingUser.IsDeleted)
            {
                return Conflict(new
                {
                    Status = 409,
                    Message = "The email address is already registered"
                });
            }
                return BadRequest("The user cannot be added");
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("Delete/{userid}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(string userid)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userid);
                if (user==null)
                {
                    return BadRequest();
                }
                user.IsDeleted= true;
                var result = await userManager.UpdateAsync(user);
                if (result ==null)
                {
                    return BadRequest();
                }

                return Ok(new ApiResponse<string>
                {
                    Data ="Done",
                    Message ="Done"

                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<string>>
                {
                    Message = ex.Message,

                });

            }

        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        /*        [HttpPatch("Edit")]
                public async Task<ActionResult<ApiResponse<string>>> Save([FromForm]VmEditUser model)
                {
                    ApiResponse<List<string>> response = new ApiResponse<List<string>>();
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(response.Message ="validate Error");
                    }

                    var user = await userManager.FindByIdAsync(model.userid);
                    if (user == null)
                    {
                        return NotFound(response.Message = "Not Found");
                    }

                    user.UserName = model.username;
                    user.Email = model.Email;



                    var currentRoles = await userManager.GetRolesAsync(user);
                    var selectedRole = model.Roles;

                    foreach (var role in currentRoles)
                    {
                        await userManager.RemoveFromRoleAsync(user, role);
                    }

                    if (!string.IsNullOrEmpty(selectedRole))
                    {
                        await userManager.AddToRoleAsync(user, selectedRole);
                    }

                    var result = await  userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok(response.Message = "Done") ;
                    }


                    return Ok( response);
                }*/
        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Save([FromForm] VmEditUser model)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            if (!ModelState.IsValid)
                return BadRequest(response.Message = "Validate Error");

            var user = await userManager.FindByIdAsync(model.userid);
            if (user == null)
                return NotFound(response.Message = "Not Found");

            user.UserName = model.username;
            user.Email = model.Email;

            // تعديل كلمة المرور إن وُجدت
            if (!string.IsNullOrWhiteSpace(model.password))
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var passwordResult = await userManager.ResetPasswordAsync(user, token, model.password);
                if (!passwordResult.Succeeded)
                {
                    return BadRequest(passwordResult.Errors.Select(e => e.Description));
                }
            }

            // تعديل الأدوار
            var currentRoles = await userManager.GetRolesAsync(user);
            foreach (var role in currentRoles)
                await userManager.RemoveFromRoleAsync(user, role);

            if (!string.IsNullOrEmpty(model.Roles))
                await userManager.AddToRoleAsync(user, model.Roles);

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok(response.Message = "Done");

            return BadRequest(result.Errors.Select(e => e.Description));
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
            var employee = await CTX.TbEmployees.FirstOrDefaultAsync(e => e.UserId == user.Id);
            if (employee != null)
            {
                claims.Add(new Claim("EmployeeId", employee.Id.ToString()));
            }
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
                expires: DateTime.Now.AddHours(480),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}