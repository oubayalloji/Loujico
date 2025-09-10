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
        IFiles ClsFiles;
      
        CompanySystemContext CTX;
        UserManager<ApplicationUser> UserManager;
        public ProductController(IProducts clsProducts, CompanySystemContext context, UserManager<ApplicationUser> userManager, IHistory clsHistory, IFiles clsFiles,Ilog clsLogs)
        {
            ClsProducts = clsProducts;
            CTX = context;
            UserManager = userManager;
            ClsHistory = clsHistory;
            ClsFiles = clsFiles;
            ClsHistory = clsHistory;
            ClsLogs = clsLogs;
        }
        [HttpPost("Add")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] TbProduct prod, [FromForm] List<FileModel>? Data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> { Message = "wronge" });
            }

            try
            {
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                prod.CreatedBy = username;
                await ClsProducts.Add(prod);
                await ClsLogs.Add("Error", $"{prod.Id} added to the System by {username}", userId);
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Products", prod.Id, tableName.product);
                    }
                }
                return Ok(new ApiResponse<string> { Message = "Done" });
            }

            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ApiResponse<ProductModel>>> GetById(int id)
        {
            try
            {
                var invoice = await ClsProducts.GetById(id);
                return Ok(new ApiResponse<ProductModel> { Data = invoice });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<InvoiceModel> { Message = ex.Message });
            }
        }
        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromBody] TbProduct Product, [FromForm] List<FileModel>? Data)
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
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Products", Product.Id, tableName.product);
                        await ClsLogs.Add("CRUD", $"file {item.fileType} added to : {Product.ProductName} by {username} ", userId);

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
                return BadRequest(new ApiResponse<List<TbProduct>>
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
                var history = await ClsProducts.LstEditHistory(page, id, count);
                return Ok(new ApiResponse<List<TbHistory>> { Data = history });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbHistory>> { Message = ex.Message });
            }
        }
        [HttpDelete("DeleteImg/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteFile(int id)
        {
            try
            {
               var file= await ClsFiles.GetById(id,tableName.product);
                await ClsFiles.Delete(id,tableName.product);

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
                var Product = await ClsProducts.GetById(id);
                await ClsProducts.Delete(id);

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("Error", $"{Product.Product.ProductName} Deleted from the System by {username} ", userId);
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
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<List<TbProduct>>>> GetAllEmployees([FromQuery] int Page, [FromQuery] int Count)
        {

            try
            {
                var Products = await ClsProducts.GetAllProducts(Page,Count);
                if (Products==null)
                {
                    return NotFound(new ApiResponse<List<TbProduct>> { Message ="There is no projects"});
                }
                return Ok(new ApiResponse<List<TbProduct>>
                {
                    Data = Products
                }) ;
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
    }
}
