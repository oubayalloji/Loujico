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

    public class ProductController : ControllerBase
    {
        IProducts ClsProducts;
        Ilog ClsLogs;
        IHistory ClsHistory;
        CompanySystemContext CTX;
        UserManager<ApplicationUser> UserManager;
        public ProductController(IProducts ClsProducts, CompanySystemContext context, UserManager<ApplicationUser> userManager, IHistory clsHistory)
        {
            ClsProducts = ClsProducts;
            CTX = context;
            UserManager = userManager;
            ClsHistory = clsHistory;
        }
        [HttpPost("Add")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromForm] TbProduct prod)
        {
            try
            {
                if (!ModelState.IsValid)
                {

                    return BadRequest(new ApiResponse<String>
                    {

                        Message = "wronge"


                    });

                }
                if (await ClsProducts.Add(prod))
                {
                    return Ok(new ApiResponse<String>
                    {

                        Message = "Product added successfully"

                    });
                }
                else
                {
                    return Ok(new ApiResponse<String>
                    {

                        Message = "failed"

                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        public ActionResult<ApiResponse<TbProduct>> Show(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {

                    return BadRequest(new ApiResponse<String>
                    {
                        Data = "wronge",
                        Message = "wronge"

                    });

                }
                return BadRequest(new ApiResponse<String>
                {
                    Data = "wronge",
                    Message = "wronge"

                });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromForm] TbProduct Product)
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
                Product.UpdatedBy = username;
                await ClsProducts.Edit(Product);

                await ClsLogs.Add("Error", $"id : {Product.Id} with name : {Product.ProductName} updated to the System by {username} ", userId);

                return Ok(new ApiResponse<String>
                {
                 
                    Message = "Done"

                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbProduct>>
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
                var history = await ClsProducts.LstEditHistory(page, id);
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
