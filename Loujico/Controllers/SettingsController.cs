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
                    return NotFound(new ApiResponse<string> { Message = "There is no Legals" });
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
        [HttpGet("GetAllActivityType")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllActivityType()
        {

            try
            {
                var Activity = await ClsSettings.GetAllActivityType();
                if (Activity == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no Activities" });
                return Ok(new ApiResponse<List<object>>
                {
                    Data = Activity
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
        public async Task<ActionResult<ApiResponse<string>>> AddIndustry([FromForm] string Name)
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
                Co_Industry co_Industry = new Co_Industry();
                co_Industry.Name = Name;
                await ClsSettings.AddIndustry(co_Industry);
                // من هون 
                await ClsLogs.Add("CRUD", $"{co_Industry.Name} added to the System by {username} ", userId);
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
        public async Task<ActionResult<ApiResponse<string>>> AddContact([FromForm] string name)
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
                TbContact contact = new TbContact();
                contact.Name = name;
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);

                await ClsSettings.AddContact(contact);
                // من هون 
                await ClsLogs.Add("CRUD", $"{contact.Name} added to the System by {username} ", userId);
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
        public async Task<ActionResult<ApiResponse<string>>> AddLegal([FromForm] string name)
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
                Co_Legal co_Legal = new Co_Legal();
                co_Legal.LegalInfo = name;
                if(!await ClsSettings.AddLegal(co_Legal))
                {
                    return Ok(new ApiResponse<string>
                    {

                        Message = "Error"

                    });
                }
                // من هون 
                await ClsLogs.Add("CRUD", $"{co_Legal.LegalInfo} added to the System by {username} ", userId);
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
        [HttpPost("AddActivity")]
        public async Task<ActionResult<ApiResponse<string>>> AddActivity([FromForm] string name, [FromForm] int IndustryId)
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
                Co_Activity co_Activity = new Co_Activity();
                co_Activity.Name = name;
                co_Activity.IndustryId = IndustryId;
                if(!await ClsSettings.AddActivity(co_Activity))
                {
                    return Ok(new ApiResponse<string>
                    {

                        Message = "Error"

                    });
                }
                // من هون 
                await ClsLogs.Add("CRUD", $"{co_Activity.Name} added to the System by {username} ", userId);
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
        [HttpDelete("DeleteActivity/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteActivity(int id)
        {
            try
            {
                var Cont = CTX.TbContact.FirstOrDefault(x => x.Id == id);
                var Emp = await ClsSettings.DeleteActivity(id);
                if (Emp == false)
                {
                    return NotFound(new ApiResponse<List<TbEmployee>>
                    {
                        Message = "النشاط غير موجد",
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
        [HttpGet("GetActivityByIndustry/{id}")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetActivityByIndustry(int id)
        {

            try
            {
                var ActivityByIndustry = await ClsSettings.GetActivityByIndustry(id);
                if (ActivityByIndustry == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no ActivityByIndustry" });
                return Ok(new ApiResponse<List<object>>
                {
                    Data = ActivityByIndustry
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
    }
}
