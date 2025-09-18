using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public ProductController(IProducts clsProducts, CompanySystemContext context, UserManager<ApplicationUser> userManager, IHistory clsHistory, IFiles clsFiles, Ilog clsLogs)
        {
            ClsProducts = clsProducts;
            CTX = context;
            UserManager = userManager;
            ClsHistory = clsHistory;
            ClsFiles = clsFiles;
            ClsHistory = clsHistory;
            ClsLogs = clsLogs;
            UserManager = userManager;
        }
        [HttpPost("Add")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromForm] AddProductModel dto, [FromForm] List<FileModel>? Data)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var username = UserManager.GetUserName(User);
                var product = new TbProduct
                {
                    ProductName = dto.ProductName,
                    ProductDescription = dto.ProductDescription,
                    BillingCycle = dto.BillingCycle,
                    Price = dto.Price,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now,
                    CreatedBy = username,
                   

                };

                // ربط الموظفين بالمشروع
                foreach (var emp in dto.Employees)
                {
                    product.TbProductsEmployees.Add(new TbProductsEmployee
                    {
                        EmployeeId = emp.EmployeeId,
                        RoleOnProduct = emp.RoleOnProject,
                        JoinedAt = DateTime.Now
                    });
                }

                CTX.TbProducts.Add(product);
                await CTX.SaveChangesAsync();
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Products", product.Id, tableName.product);
                    }
                }
                var usename = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"{dto.ProductName} Added to the System by {usename} ", userId);

                return Ok(new { product.Id, message = "تمت إضافة المشروع بنجاح" });
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

    

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ApiResponse<ProductModel>>> GetById(int id)
        {
            try
            {
                var Product = await ClsProducts.GetById(id);
                return Ok(new ApiResponse<ProductModel> { Data = Product });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<ProductModel> { Message = ex.Message });
            }
        }
        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromForm] AddProductModel dto, [FromForm] List<FileModel>? Data)
        {

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Message = "Invalid payload." });

            // جلب المشروع مع علاقات الموظفين (المحذوفين يُستبعدون تلقائياً عبر HasQueryFilter)
            var prod = await CTX.TbProducts
                .Include(p => p.TbProductsEmployees)
                .FirstOrDefaultAsync(p => p.Id == dto.Id && !p.IsDeleted);

            if (prod == null)
                return NotFound(new ApiResponse<string> { Message = "product not found." });

            var username = UserManager.GetUserName(User);
            var userId = UserManager.GetUserId(User);

            // تحديث خصائص المشروع
            prod.ProductName = dto.ProductName;
            prod.ProductDescription = dto.ProductDescription;
            prod.BillingCycle = dto.BillingCycle;
            prod.Price = dto.Price;

            prod.IsActive = dto.IsActive;
            prod.UpdatedAt = DateTime.Now;
            prod.UpdatedBy = username;

            // 1. علّم جميع روابط الموظفين الحالية محذوفة
            foreach (var link in prod.TbProductsEmployees)
            {
                link.IsDeleted = true;
            }

            // 2. عُد تفعيل أو أضف الروابط الواردة في dto.Employees
            foreach (var empDto in dto.Employees ?? Enumerable.Empty<EmployeeOnProjectModel>())
            {
                var match = prod.TbProductsEmployees
                    .FirstOrDefault(pe =>
                        pe.EmployeeId == empDto.EmployeeId &&
                        pe.RoleOnProduct == empDto.RoleOnProject);

                if (match != null)
                {
                    // إعادة التفعيل وتحديث وقت الانضمام
                    match.IsDeleted = false;
                    match.JoinedAt = DateTime.Now;
                }
                else
                {
                    // إضافة سجل جديد للموظف
                    prod.TbProductsEmployees.Add(new TbProductsEmployee
                    {
                        EmployeeId = empDto.EmployeeId,
                        RoleOnProduct = empDto.RoleOnProject,
                        JoinedAt = DateTime.Now,
                        IsDeleted = false
                    });
                }
            }

            // حفظ التعديلات دفعة واحدة
            await CTX.SaveChangesAsync();

            // تسجيل السجلّات
            await ClsLogs.Add("CRUD", $"product '{prod.ProductName}' updated by {username}.", userId);

            // معالجة الملفات إن وجدت
            if (Data != null)
            {
                foreach (var file in Data)
                {
                    await ClsFiles.Add(file, "products", prod.Id, tableName.product);
                    await ClsLogs.Add(
                        "CRUD",
                        $"File '{file.fileType}' added to Product '{prod.ProductName}' by {username}.",
                        userId);
                }
            }

            return Ok(new ApiResponse<string> { Message = "Done" });
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
        [HttpDelete("DeleteFile/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteFile(int id)
        {
            try
            {
               var file= await ClsFiles.GetById(id,tableName.product);
                await ClsFiles.Delete(id,tableName.product);

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"file {file.FileType} for {file.EntityId} in table{file.EntityType} Deleted from the System by {username} ", userId);
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
                await ClsLogs.Add("CRUD", $"{Product.Product.ProductName} Deleted from the System by {username} ", userId);
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

        [HttpGet("Search")]
        public async Task<ActionResult<ApiResponse<object>>> Search([FromQuery] string name, [FromQuery] int page, [FromQuery] int count)
        {
            try
            {
                var Product = await ClsProducts.Search(name, page, count);
                if (Product == null)
                {
                    return NotFound(new ApiResponse<object> { Message = "No result" });
                }
                return Ok(new ApiResponse<object>
                {
                    Data = Product
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
        [HttpGet("GetCount")]
        public async Task<ActionResult<ApiResponse<int>>> GetCount()
        {
            try
            {
                var Product = await ClsProducts.Count();
                if (Product == 0 || Product == null)
                {
                    return NotFound(new ApiResponse<int> { Message = "There is no products" });
                }

                return Ok(new ApiResponse<int>
                {
                    Data = Product
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
