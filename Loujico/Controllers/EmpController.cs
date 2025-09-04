using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Loujico.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmpController : ControllerBase
    {
        CompanySystemContext CTX;
        IEmployees ClsEmployees;
        Ilog ClsLogs;
        UserManager<ApplicationUser> UserManager;
        public EmpController(CompanySystemContext cTX, IEmployees clsEmployees, Ilog clsLogs, UserManager<ApplicationUser> userManager)
        {

            CTX = cTX;
            ClsEmployees = clsEmployees;
            ClsLogs = clsLogs;
            UserManager = userManager;
        }
        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromForm] TbEmployee emp)
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
                return Ok(new ApiResponse<String>
                {
                   Success=true,
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
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromForm] TbEmployee emp)
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
                    Success = true,
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
                var Emp =  await ClsEmployees.GetEmployeeById(id);

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("Error", $"{Emp.FirstName} Deleted from the System by {username} ", userId);
                // لهون هو تسجيل الlog  
                return Ok(new ApiResponse<String>
                {
                    Success = true,
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
                var Employee = await ClsEmployees.GetEmployeeById(id);

                return Ok(new ApiResponse<TbEmployee>
                {
                    Success = true,
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
    }
}