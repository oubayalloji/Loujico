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
        IFiles ClsFiles;
        public CustomerController(CompanySystemContext cTX, ICustomers clsCustomers, Ilog clsLogs, UserManager<ApplicationUser> userManager, IHistory clsHistory, IFiles clsFiles)
        {

            CTX = cTX;
            ClsCustomers = clsCustomers;
            ClsLogs = clsLogs;
            UserManager = userManager;
            ClsHistory = clsHistory;
            ClsFiles = clsFiles;
        }
        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] TbCustomer Customer, [FromForm] List<FileModel>? Data)
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
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Customers", Customer.Id, tableName.Customer);
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
                return BadRequest(new ApiResponse<List<TbCustomer>>
                {
                    Message = ex.Message,
                });
            }
        }

        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromBody] TbCustomer Customer, [FromForm] List<FileModel>? Data)
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
                var vCustomer = await ClsCustomers.GetById(Customer.Id);
                if (vCustomer == null)
                    return NotFound(new ApiResponse<string> { Message = "the Customer is deleted or not found " });
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                Customer.UpdatedBy = username;
                await ClsCustomers.Edit(Customer);
                await ClsLogs.Add("Error", $"id : {Customer.Id} with name :{Customer.CustomerName} updated to the System by {username} ", userId);
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Customers", Customer.Id, tableName.Customer);
                        await ClsLogs.Add("CRUD", $"file {item.fileType} added to : {Customer.CustomerName} by {username} ", userId);

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
                return BadRequest(new ApiResponse<List<TbCustomer>>
                {
                    Message = ex.Message,

                });
            }
        }

        [HttpGet("GetAllCustomersId")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllCustomersId()
        {

            try
            {
                var Customer = await ClsCustomers.GetAllCustomersIdAndName();
                if (Customer == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no Customers" });
                return Ok(new ApiResponse<List<object>>
                {
                    Data = Customer
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<object>>
                {
                    Message = ex.Message,

                });

            }
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<List<TbCustomer>>>> GetAll([FromQuery] int Page, [FromQuery] int Count)
        {

            try
            {
                var Customer = await ClsCustomers.GetAll(Page, Count);
                if (Customer == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no Customers" });
                return Ok(new ApiResponse<List<TbCustomer>>
                {
                    Data = Customer
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
        [HttpDelete("DeleteFile/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteFile(int id)
        {
            try
            {
                var file = await ClsFiles.GetById(id, tableName.Customer);
                if (file == null)
                    return NotFound(new ApiResponse<string> { Message = "the file is deleted or not found " });
                await ClsFiles.Delete(id, tableName.Customer);

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
                return BadRequest(new ApiResponse<List<TbProject>>
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
                var Customer = await ClsCustomers.GetById(id);
                if (Customer == null)
                    return NotFound(new ApiResponse<string> { Message = "the field is deleted or not found " });
                await ClsCustomers.Delete(id);
                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("Error", $"{Customer.Customer.CustomerName} Deleted from the System by {username} ", userId);
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
                var Customer = await ClsCustomers.GetById(id);
                if (Customer == null)
                {
                    return NotFound(new ApiResponse<object> { Message = "Customer is deleted or could not found" });
                }

                return Ok(new ApiResponse<CustomerModel>
                {
                    Data = Customer
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

        [HttpGet("Search")]
        public async Task<ActionResult<ApiResponse<object>>> Search([FromQuery] string name, [FromQuery] int page, [FromQuery] int count)
        {
            try
            {
                var Customer = await ClsCustomers.Search(name, page, count);
                if (Customer == null)
                {
                    return NotFound(new ApiResponse<object> { Message = "No result" });
                }
                return Ok(new ApiResponse<object>
                {
                    Data = Customer
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
        [HttpGet("EditHistory")]
        public async Task<ActionResult<ApiResponse<List<TbHistory>>>> LstEditHistory([FromQuery] int page, [FromQuery] int id, [FromQuery] int count)
        {
            try
            {
                var history = await ClsCustomers.LstEditHistory(page, id, count);
                if (history == null)
                {
                    return NotFound(new ApiResponse<object> { Message = "There is No edit History" });
                }
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