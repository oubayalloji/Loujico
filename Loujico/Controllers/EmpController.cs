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
        public async Task<ActionResult<ApiResponse<string>>> Add([FromForm] TbEmployee emp, [FromForm]  List<FileModel>? Data )
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

        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromBody] TbEmployee emp)
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
                await ClsLogs.Add("Error", $"{emp.FirstName} updated to the System by {username} ", userId);
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

        [HttpGet("GetAllEmployees/{id}")]
        public async Task<ActionResult<ApiResponse<List<TbEmployee>>>> GetAllEmployees(int id)
        {

            try
            {

                return Ok(new ApiResponse<List<TbEmployee>>
                {
                    Data = await ClsEmployees.GetAllEmployees(id)
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
        [HttpDelete("Delete")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                await ClsEmployees.Delete(id);
                var Emp =  await ClsEmployees.GetById(id);

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
        }  [HttpGet("Get")]
        public async Task<ActionResult<ApiResponse<List<TbHistory>>>> Get()
        {
            try
            {
            var emp=     CTX.TbHistories.ToList();
                return Ok(new  ApiResponse < List < TbHistory >>
                {
                    Data = emp
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
        [HttpGet("EditHistory/{page}/{id}")]
        public async Task<ActionResult<ApiResponse<List<TbHistory>>>> LstEditHistory(int page, int id)
        {
            try
            {
                var history = await ClsEmployees.LstEditHistory(page, id);
                return Ok(new ApiResponse<List<TbHistory>> { Data = history });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbHistory>> { Message = ex.Message });
            }
        }
    }
}
