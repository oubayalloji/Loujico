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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

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
        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] TbEmployee emp, [FromForm]  List<FileModel>? Data )
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
                await ClsLogs.Add("Error", $"{emp.FirstName} added to the System by {username} ", userId);

                // لهون هو تسجيل الlog
                if (Data!= null)
                {
                    foreach (var item in Data)
                    {
                      await  ClsFiles.Add(item, "Employees", emp.Id, tableName.Employee);
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
        [HttpGet("GetAllEmployeesIdAndName")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllEmployeesIdAndName()
        {

            try
            {
                var Employee = await ClsEmployees.GetAllEmployeesIdAndName();
                if(Employee==null)
                    return NotFound(new ApiResponse<string> { Message="There is no Employees"});
                return Ok(new ApiResponse<List<object>>
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

        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromBody] TbEmployee emp, [FromForm] List<FileModel>? Data)
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
                await ClsLogs.Add("Error", $"{emp.FirstName} {emp.LastName} updated to the System by {username} ", userId);
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Employees", emp.Id, tableName.Employee);
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


        }

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
                await ClsLogs.Add("Error", $"{Emp.Employee.FirstName} Deleted from the System by {username} ", userId);
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
        [HttpDelete("DeleteImg/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteFile(int id)
        {
            try
            {
                var file = await ClsFiles.GetById(id, tableName.Employee);
                await ClsFiles.Delete(id, tableName.Employee);

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("Error", $"file {file.FileType} for {file.EntityId} in table{file.EntityType} Deleted from the System by {username} ", userId);
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
