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
        [HttpPost("AddEmp")]
        public async Task<ActionResult<ApiResponse<string>>> AddEmp([FromForm] TbEmployee emp)
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
                CTX.TbEmployees.Add(emp);
                await CTX.SaveChangesAsync();
                return Ok(new ApiResponse<String>
                {
                    Data = "Done",
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
                var userid = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", "Delete", userid);
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
                var Employee = await ClsEmployees.GetEmployeeById(id);

                return Ok(new ApiResponse<TbEmployee>
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
    }
}