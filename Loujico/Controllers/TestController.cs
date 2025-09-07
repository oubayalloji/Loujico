using Loujico.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loujico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<ApiResponse<String>> GetUser(int id)
        {
     

            return Ok(new ApiResponse<String>
            {
            
                Message = "تم جلب المستخدم بنجاح",
                Data = "Hello"
            });
        }

    }
}
