using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Loujico.Controllers
{
  

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class EmpController : ControllerBase
    {
        CompanySystemContext CTX;
        IEmployees ClsEmployees;
        Ilog ClsLogs;
        IHistory ClsHistory;
        IFiles ClsFiles;
        UserManager<ApplicationUser> UserManager;
        public EmpController(CompanySystemContext cTX, IEmployees clsEmployees, Ilog clsLogs, UserManager<ApplicationUser> userManager, IHistory clsHistory, IFiles clsFiles)
        {
            CTX = cTX;
            ClsEmployees = clsEmployees;
            ClsLogs = clsLogs;
            UserManager = userManager;
            ClsHistory = clsHistory;
            ClsFiles = clsFiles;
        }
        /* [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
         [HttpPost("Add")]
         public async Task<ActionResult<ApiResponse<string>>> Add([FromForm] TbEmployee emp, [FromForm] List<FileModel>? Data)
         {

             if (!ModelState.IsValid)
             {

                 return BadRequest(new ApiResponse<String>
                 {

                     Message = "wronge"

                 });

             }
             try
             {

                 var username = UserManager.GetUserName(User);
                 var userId = UserManager.GetUserId(User);
                 emp.CreatedBy = username;
                 await ClsEmployees.Add(emp);
                 // من هون 
                 await ClsLogs.Add("CRUD", $"{emp.FirstName} added to the System by {username} ", userId);

                 // لهون هو تسجيل الlog
                 if (Data != null)
                 {
                     foreach (var item in Data)
                     {
                         await ClsFiles.Add(item, "Employees", emp.Id, tableName.Employee,username);
                     }
                 }
                 return Ok(new ApiResponse<String>
                 {

                     Message = "Done"

                 });
             }
             catch (Exception ex)
             {
                 await ClsLogs.Add("Error", ex.Message, null);
                 return BadRequest(new ApiResponse<List<TbEmployee>>
                 {
                     Message = ex.Message,

                 });
             }


         }
 */
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] TbEmployee emp, [FromForm] List<FileModel>? Data)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Message = "wronge" });

            try
            {
                // إذا تم تمرير UserId، نتحقق أولاً أنه فعلاً موجود في AspNetUsers (اختياري لكن موصى به)
                if (!string.IsNullOrWhiteSpace(emp.UserId))
                {
                    var aspUser = await UserManager.FindByIdAsync(emp.UserId);
                    if (aspUser == null)
                        return BadRequest(new ApiResponse<string> { Message = "UserId غير موجود في نظام المصادقة." });

                    // تحقق من عدم وجود موظف آخر بنفس UserId
                    var exists = await CTX.TbEmployees.AnyAsync(e => e.UserId == emp.UserId);
                    if (exists)
                        return BadRequest(new ApiResponse<string> { Message = "هذا UserId مرتبط بموظف آخر. لا يمكن تكرار UserId." });
                }

                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                emp.CreatedBy = username;

                await ClsEmployees.Add(emp);

                await ClsLogs.Add("CRUD", $"{emp.FirstName} added to the System by {username} ", userId);

                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Employees", emp.Id, tableName.Employee, username);
                    }
                }

                return Ok(new ApiResponse<string> { Message = "Done" });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Team_Leader")]
        [HttpGet("GetAllId")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllId([FromQuery]bool? userid)
        {

            try
            {
                if (userid==null)
                {
                    
                var Employee = await ClsEmployees.GetAllEmployeesIdAndName();
                if(Employee==null)
                    return NotFound(new ApiResponse<string> { Message="There is no Employees"});
                return Ok(new ApiResponse<List<object>>
                {
                    Data = Employee
                });
                }
                else
                {
                    var Employee = await ClsEmployees.GetAllEmployeesIdUserid();
                    if (Employee == null)
                        return NotFound(new ApiResponse<string> { Message = "There is no Employees" });
                    return Ok(new ApiResponse<List<object>>
                    {
                        Data = Employee
                    });
                }
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });

            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("GetCount")]
        public async Task<ActionResult<ApiResponse<int>>> GetCount()
        {
            try
            {
                var Employee = await ClsEmployees.Count();
                if (Employee == 0 || Employee == null)
                {
                    return Ok(new ApiResponse<int> { Message = "There is no employees" ,Data =0});
                }

                return Ok(new ApiResponse<int>
                {
                    Data = Employee
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<int>>
                {
                    Message = ex.Message,

                });
            }

        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromForm] TbEmployee emp, [FromForm] List<FileModel>? Data)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Message = "wronge" });

            try
            {
                // إذا تم تمرير UserId، نتحقق أنه ليس مخصّص لموظف آخر (باستثناء هذا الموظف نفسه)
                if (!string.IsNullOrWhiteSpace(emp.UserId))
                {
                    var aspUser = await UserManager.FindByIdAsync(emp.UserId);
                    if (aspUser == null)
                        return BadRequest(new ApiResponse<string> { Message = "UserId غير موجود في نظام المصادقة." });

                    var existsOther = await CTX.TbEmployees
                        .AnyAsync(e => e.UserId == emp.UserId && e.Id != emp.Id);

                    if (existsOther)
                        return BadRequest(new ApiResponse<string> { Message = "هذا UserId مرتبط بموظف آخر. لا يمكن تكرار UserId." });
                }

                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                emp.UpdatedBy = username;

                await ClsEmployees.Edit(emp);

                await ClsLogs.Add("CRUD", $"{emp.FirstName} {emp.LastName} updated to the System by {username} ", userId);

                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Employees", emp.Id, tableName.Employee, username);
                        await ClsLogs.Add("CRUD", $"file {item.fileType} added to : {emp.FirstName} {emp.LastName} by {username} ", userId);
                    }
                }

                return Ok(new ApiResponse<string> { Message = "Done" });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }

        /*  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
          [HttpPatch("Edit")]
          public async Task<ActionResult<ApiResponse<string>>> Edit([FromForm] TbEmployee emp, [FromForm] List<FileModel>? Data)
          {

              if (!ModelState.IsValid)
              {

                  return BadRequest(new ApiResponse<String>
                  {

                      Message = "wronge"

                  });

              }
              try
              {
                  var username = UserManager.GetUserName(User);
                  var userId = UserManager.GetUserId(User);
                  emp.UpdatedBy = username;
                  await ClsEmployees.Edit(emp);
                  // من هون 
                  await ClsLogs.Add("CRUD", $"{emp.FirstName} {emp.LastName} updated to the System by {username} ", userId);
                  if (Data != null)
                  {
                      foreach (var item in Data)
                      {
                          await ClsFiles.Add(item, "Employees", emp.Id, tableName.Employee,username);
                          await ClsLogs.Add("CRUD", $"file {item.fileType} added to : {emp.FirstName} {emp.LastName} by {username} ", userId);

                      }
                  }
                  // لهون هو تسجيل الlog
                  return Ok(new ApiResponse<String>
                  {

                      Message = "Done"

                  });
              }
              catch (Exception ex)
              {
                  await ClsLogs.Add("Error", ex.Message, null);
                  return BadRequest(new ApiResponse<List<TbEmployee>>
                  {
                      Message = ex.Message,

                  });
              }


          }*/
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<List<TbEmployee>>>> GetAllEmployees([FromQuery] int Page, [FromQuery] int Count)
        {

            try
            {

                return Ok(new ApiResponse<List<TbEmployee>>
                {
                    Data = await ClsEmployees.GetAllEmployees(Page,Count)
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });

            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                var Emp =  await ClsEmployees.GetById(id);
                if (Emp == null)
                {
                    return NotFound(new ApiResponse<List<TbEmployee>>
                    {
                        Message = "الموظف غير موجد",
                    });
                }
                await ClsEmployees.Delete(id);

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"{Emp.Employee.FirstName} Deleted from the System by {username} ", userId);
                // لهون هو تسجيل الlog  
                return Ok(new ApiResponse<String>
                {
                  
                    Data = "done"
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("DeleteFile/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteFile(int id)
        {
            try
            {
                var file = await ClsFiles.GetById(id, tableName.Employee);
                await ClsFiles.Delete(id, tableName.Employee);

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"file {file.FileType} for {file.EntityId} in table{file.EntityType} Deleted from the System by {username} ", userId);
                // لهون هو تسجيل الlog  
                return Ok(new ApiResponse<String>
                {
                    Data = "done"
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });
            }

        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> GetById(int id)
        {
            try
            {
                var Employee = await ClsEmployees.GetById(id);

                return Ok(new ApiResponse<ShowEmployeeModel>
                {
                    Data = Employee
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("EditHistory")]
        public async Task<ActionResult<ApiResponse<List<TbHistory>>>> LstEditHistory([FromQuery] int page, [FromQuery] int id, [FromQuery] int Count)
        {
            try
            {
                var history = await ClsEmployees.LstEditHistory(page, id,Count);
                return Ok(new ApiResponse<List<TbHistory>> { Data = history });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbHistory>> { Message = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("Search")]
        public async Task<ActionResult<ApiResponse<object>>> Search([FromQuery] string name, [FromQuery] int page, [FromQuery] int count)   
        {
            try
            {
                var Customerloyee = await ClsEmployees.Search(name, page, count);

                return Ok(new ApiResponse<object>
                {
                    Data = Customerloyee
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });
            }

        }

    }
}
