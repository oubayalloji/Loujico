using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Loujico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

    public class CustomerController : ControllerBase
    {
        CompanySystemContext CTX;
        ICustomers ClsCustomers;
        Ilog ClsLogs;
        IHistory ClsHistory;
        UserManager<ApplicationUser> UserManager;
        public CustomerController(CompanySystemContext cTX, ICustomers clsCustomers, Ilog clsLogs, UserManager<ApplicationUser> userManager, IHistory clsHistory)
        {

            CTX = cTX;
            ClsCustomers = clsCustomers;
            ClsLogs = clsLogs;
            UserManager = userManager;
            ClsHistory = clsHistory;
        }
        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromForm] TbCustomer Customer)
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
                Customer.CreatedBy = username;
                await ClsCustomers.Add(Customer);
                // من هون 
                await ClsLogs.Add("Error", $"{Customer.CustomerName} added to the System by {username} ", userId);
                // لهون هو تسجيل الlog
                return Ok(new ApiResponse<String>
                {
                    Message = "Done"

                });

            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbCustomer>>
                {
                    Message = ex.Message,
                });
            }
        }

        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromForm] TbCustomer Customer)
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
                Customer.UpdatedBy = username;
                await ClsCustomers.Edit(Customer);
                await ClsLogs.Add("Error", $"id : {Customer.Id} with name :{Customer.CustomerName} updated to the System by {username} ", userId);
                return Ok(new ApiResponse<String>
                {

                    Message = "Done"

                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbCustomer>>
                {
                    Message = ex.Message,

                });
            }
        }

        [HttpGet("GetAll/{id}")]
        public async Task<ActionResult<ApiResponse<List<TbCustomer>>>> GetAll(int id)
        {

            try
            {

                return Ok(new ApiResponse<List<TbCustomer>>
                {
                    Data = await ClsCustomers.GetAll(id)
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbCustomer>>
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
                var Customer = await ClsCustomers.GetById(id);
                await ClsCustomers.Delete(id);
                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("Error", $"{Customer.CustomerName} Deleted from the System by {username} ", userId);
                // لهون هو تسجيل الlog  
                return Ok(new ApiResponse<String>
                {
                    Data = "done"
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbCustomer>>
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
                var Customerloyee = await ClsCustomers.GetById(id);

                return Ok(new ApiResponse<TbCustomer>
                {
                    Data = Customerloyee
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbCustomer>>
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
                var history = await ClsCustomers.LstEditHistory(page, id);
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