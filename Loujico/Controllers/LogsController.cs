using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Loujico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class LogsController : ControllerBase
    {
        CompanySystemContext CTX;
        Ilog ClsLogs;
        UserManager<ApplicationUser> UserManager;
        public LogsController(CompanySystemContext cTX, Ilog clsLogs, UserManager<ApplicationUser> userManager)
        {
            CTX = cTX;
            ClsLogs = clsLogs;
            UserManager = userManager;
        }
        //[HttpGet("Search")]
        //public async Task<ActionResult<ApiResponse<object>>> Search([FromQuery] string name, [FromQuery] int page, [FromQuery] int count)
        //{
        //    try
        //    {
        //        var Invoice = await ClsLogs.Search(name, page, count);
        //        if (Invoice == null)
        //        {
        //            return NotFound(new ApiResponse<object> { Message = "No result" });
        //        }
        //        return Ok(new ApiResponse<object>
        //        {
        //            Data = Invoice
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        await ClsLogs.Add("Error", ex.Message, null);
        //        return BadRequest(new ApiResponse<List<TbInvoice>>
        //        {
        //            Message = ex.Message,

        //        });
        //    }

        //}
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<List<TbLog>>>> Paginition([FromQuery] int Page, [FromQuery] int Count)
        {

            try
            {
                var Log = await ClsLogs.Paginition(Page, Count);
                if (Log == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no Logs" });
                return Ok(new ApiResponse<List<TbLog>>
                {
                    Data = Log
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbLog>>
                {
                    Message = ex.Message,

                });

            }
        }
        [HttpGet("GetCount")]
        public async Task<ActionResult<ApiResponse<int>>> GetCount()
        {
            try
            {
                var Log = await ClsLogs.Count();
                if (Log == 0 || Log == null)
                {
                    return NotFound(new ApiResponse<int> { Message = "There is no Logs" });
                }

                return Ok(new ApiResponse<int>
                {
                    Data = Log
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
    }
}
