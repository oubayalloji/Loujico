// من هون 
using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var username = UserManager.GetUserName(User);
var userId = UserManager.GetUserId(User);
await ClsLogs.Add("Error", $"{Customer.CustomerName} Deleted from the System by {username} ", userId);
// لهون هو تسجيل الlog  
return Ok(new ApiResponse<String>
{
<<<<<<< HEAD
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        CompanySystemContext CTX;
        ICustomers ClsCustomers;
        Ilog ClsLogs;
        UserManager<ApplicationUser> UserManager;
        public CustomerController(CompanySystemContext cTX, ICustomers clsCustomers, Ilog clsLogs, UserManager<ApplicationUser> userManager)
        {

            CTX = cTX;
            ClsCustomers = clsCustomers;
            ClsLogs = clsLogs;
            UserManager = userManager;
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
                
=======
    Data = "done"
});
>>>>>>> oby
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
return BadRequest(new ApiResponse<List<TbCustomer>>
{
    Message = ex.Message,

<<<<<<< HEAD
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
                await ClsLogs.Add("Error", $"{Customer.CustomerName} updated to the System by {username} ", userId);
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
=======
});
>>>>>>> oby
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
<<<<<<< HEAD
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
=======
            Data = Customerloyee
        });
>>>>>>> oby
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
    }
}