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

    public class SettingsController : ControllerBase
    {
        Isettings ClsSettings;
        Ilog ClsLogs;
        IHistory ClsHistory;
        IFiles ClsFiles;

        CompanySystemContext CTX;
        UserManager<ApplicationUser> UserManager;
        public SettingsController(Isettings clssettings, Ilog ilog, IHistory ihistory, IFiles files, UserManager<ApplicationUser> userManager, CompanySystemContext context)
        {
            ClsSettings = clssettings;
            ClsLogs = ilog;
            ClsHistory = ihistory;
            CTX = context;
            UserManager = userManager;
        }
        [HttpGet("GetAllLegalType")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllLegalType()
        {

            try
            {
                var Customer = await ClsSettings.GetAllLegalType();
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
        [HttpGet("GetAllIndustryType")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllIndustryType()
        {

            try
            {
                var Customer = await ClsSettings.GetAllIndustryType();
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
        [HttpGet("GetAllContactType")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllContactType()
        {

            try
            {
                var Customer = await ClsSettings.GetAllContactType();
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




        [HttpPost("AddIndustry")]
        public async Task<ActionResult<ApiResponse<string>>> AddIndustry([FromForm] Co_Industry emp)
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
         
                await ClsSettings.AddIndustry(emp);
                // من هون 
                await ClsLogs.Add("CRUD", $"{emp.Name} added to the System by {username} ", userId);
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
        [HttpPost("AddContact")]
        public async Task<ActionResult<ApiResponse<string>>> AddContact([FromForm] TbContact emp)
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
         
                await ClsSettings.AddContact(emp);
                // من هون 
                await ClsLogs.Add("CRUD", $"{emp.Name} added to the System by {username} ", userId);
                return Ok(new ApiResponse<string>
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
        [HttpPost("AddLegal")]
        public async Task<ActionResult<ApiResponse<string>>> AddLegal([FromForm] Co_Legal emp)
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
         
                await ClsSettings.AddLegal(emp);
                // من هون 
                await ClsLogs.Add("CRUD", $"{emp.LegalInfo} added to the System by {username} ", userId);
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



        [HttpDelete("DeleteLegal/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteLegal(int id)
        {
            try
            {
               var Cont= CTX.Co_Legals.FirstOrDefault(x => x.Id == id);
                var Emp = await ClsSettings.DeleteLegal(id);
                if (Emp == false)
                {
                    return NotFound(new ApiResponse<List<int>>
                    {
                        Message = "نوع السجل غير موجود",
                    });
                }

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"{Cont.LegalInfo} Deleted from the System by {username} ", userId);
                // لهون هو تسجيل الlog  
                return Ok(new ApiResponse<string>
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
        [HttpDelete("DeleteIndustry/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteIndustry(int id)
        {
            try
            {
               var Cont= CTX.Co_Industries.FirstOrDefault(x => x.Id == id);
                var Emp = await ClsSettings.DeleteIndustry(id);
                if (Emp == false)
                {
                    return NotFound(new ApiResponse<List<TbEmployee>>
                    {
                        Message = "وسيلة التواصل غير موجد",
                    });
                }

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"{Cont.Name} Deleted from the System by {username} ", userId);
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
        [HttpDelete("DeleteContact/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteContact(int id)
        {
            try
            {
               var Cont= CTX.TbContact.FirstOrDefault(x => x.Id == id);
                var Emp = await ClsSettings.DeleteContact(id);
                if (Emp == false)
                {
                    return NotFound(new ApiResponse<List<TbEmployee>>
                    {
                        Message = "وسيلة التواصل غير موجد",
                    });
                }

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"{Cont.Name} Deleted from the System by {username} ", userId);
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
    }
}
