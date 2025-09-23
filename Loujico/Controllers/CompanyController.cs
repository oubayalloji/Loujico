using Loujico.BL;
using Loujico.Migrations;
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

    public class CompanyController : ControllerBase
    {
        CompanySystemContext CTX;
        ICustomers ClsCustomers;
        Ilog ClsLogs;
        IHistory ClsHistory;
        UserManager<ApplicationUser> UserManager;
        IFiles ClsFiles;
        ICompanys ClsCompanys;
        public CompanyController(CompanySystemContext cTX, ICustomers clsCustomers, Ilog clsLogs, UserManager<ApplicationUser> userManager, IHistory clsHistory, IFiles clsFiles, ICompanys clsCompanys)
        {

            CTX = cTX;
            ClsCustomers = clsCustomers;
            ClsLogs = clsLogs;
            UserManager = userManager;
            ClsHistory = clsHistory;
            ClsFiles = clsFiles;
            ClsCompanys = clsCompanys;
        }
        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromForm] AddCompany dto, [FromForm] List<FileModel>? Data)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Message = "Invalid model" });

            try
            {
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);

                // إنشاء الكيان الرئيسي
                var company = new Co_Company_Name
                {
                    Name = dto.Name,
                    Comm_No = dto.Comm_No,
                    Tax_No = dto.Tax_No,
                    Found_Date = dto.Found_Date,
                    CompanyDescription = dto.CompanyDescription,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = username,
                    IsDeleted = false,
                    // تهيئة القوائم لتجنب null
                    Addresses = new List<Co_Address>(),
                    Contacts = new List<Co_Contact>(),
                    Legals = new List<Co_Legal>(),
                    Activity = new List<Co_Activity>(),
                    CompanyEmployees = new List<Co_CompanyEmployee>()
                };

                // تعبئة العناوين
                if (dto.Addresses != null)
                {
                    foreach (var a in dto.Addresses)
                    {
                        company.Addresses.Add(new Co_Address
                        {
                            CountryId = a.CountryId,
                            StateId = a.StateId,
                            CityId = a.CityId,
                            AddressLine = a.AddressLine
                        });
                    }
                }

                // تعبئة الاتصالات
                if (dto.Contacts != null)
                {
                    foreach (var c in dto.Contacts)
                    {
                        company.Contacts.Add(new Co_Contact
                        {
                            ContactTypeId = c.ContactTypeId,
                            Name = c.Name
                        });
                    }
                }

                // تعبئة الـ Legals
                if (dto.Legals != null)
                {
                    foreach (var l in dto.Legals)
                    {
                        company.Legals.Add(new Co_Legal { LegalInfo = l.LegalInfo });
                    }
                }

                // نشاطات
                if (dto.Activities != null)
                {
                    foreach (var act in dto.Activities)
                    {
                        company.Activity.Add(new Co_Activity
                        {
                            Name = act.Name,
                            IndustryId = act.IndustryId
                        });
                    }
                }

                // موظفين
                if (dto.CompanyEmployees != null)
                {
                    foreach (var emp in dto.CompanyEmployees)
                    {
                        company.CompanyEmployees.Add(new Co_CompanyEmployee
                        {
                            FirstName = emp.FirstName,
                            LastName = emp.LastName,
                            Position = emp.Position,
                            Phone = emp.Phone,
                            Email = emp.Email
                        });
                    }
                }

                // إضافة الشركة وكل الـ navigation entities عبر EF Core
                CTX.Co_Companies.Add(company);
                await CTX.SaveChangesAsync(); // الحفظ هنا يملأ company.Id و CompanyId في الكيانات المرتبطة تلقائياً

                // سجل اللوق بعد الحفظ
                await ClsLogs.Add("CRUD", $"{company.Name} added to the System by {username}", userId);

                // معالجة الملفات المرتبطة بعد أن أصبح company.Id موجوداً
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Companys", company.Id, tableName.Company);
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
        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit( [FromBody] CompanyEditDto dto, [FromForm] List<FileModel>? Data)
        {
            if (!ModelState.IsValid )
                return BadRequest(new ApiResponse<string> { Message = "Invalid model or id mismatch" });

            await using var tx = await CTX.Database.BeginTransactionAsync();
            try
            {
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);

                var company = await CTX.Co_Companies
                    .Include(c => c.Addresses)
                    .Include(c => c.Contacts)
                    .Include(c => c.Legals)
                    .Include(c => c.Activity)
                    .Include(c => c.CompanyEmployees)
                    .FirstOrDefaultAsync(c => c.Id == dto.Id && !c.IsDeleted);

                if (company == null) return NotFound(new ApiResponse<string> { Message = "Company not found" });

                // update basic fields
                company.Name = dto.Name;
                company.Comm_No = dto.Comm_No;
                company.Tax_No = dto.Tax_No;
                company.Found_Date = dto.Found_Date;
                company.CompanyDescription = dto.CompanyDescription;
                company.UpdatedAt = DateTime.UtcNow;
                company.UpdatedBy = username;

                // --- حذف كل العناصر الفرعية الحالية ---
                CTX.Co_Address.RemoveRange(company.Addresses);
                CTX.Co_Contacts.RemoveRange(company.Contacts);
                CTX.Co_Legals.RemoveRange(company.Legals);
                CTX.Co_Activities.RemoveRange(company.Activity);
                CTX.Co_CompanyEmployees.RemoveRange(company.CompanyEmployees);

                // --- إعادة الإضافة من DTOs (إن وجدت) ---
                company.Addresses = dto.Addresses?.Select(a => new Co_Address
                {
                    CountryId = a.CountryId,
                    StateId = a.StateId,
                    CityId = a.CityId,
                    AddressLine = a.AddressLine
                }).ToList() ?? new List<Co_Address>();

                company.Contacts = dto.Contacts?.Select(c => new Co_Contact
                {
                    ContactTypeId = c.ContactTypeId,
                    Name = c.Name
                }).ToList() ?? new List<Co_Contact>();

                company.Legals = dto.Legals?.Select(l => new Co_Legal
                {
                    LegalInfo = l.LegalInfo
                }).ToList() ?? new List<Co_Legal>();

                company.Activity = dto.Activities?.Select(ac => new Co_Activity
                {
                    Name = ac.Name,
                    IndustryId = ac.IndustryId
                }).ToList() ?? new List<Co_Activity>();

                company.CompanyEmployees = dto.CompanyEmployees?.Select(e => new Co_CompanyEmployee
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Position = e.Position,
                    Department = e.Department,
                    Phone = e.Phone,
                    Email = e.Email,
                    Notes = e.Notes
                }).ToList() ?? new List<Co_CompanyEmployee>();

                await CTX.SaveChangesAsync();
                await tx.CommitAsync();

                await ClsLogs.Add("CRUD", $"{company.Name} updated by {username}", userId);

                if (Data != null)
                    foreach (var item in Data) await ClsFiles.Add(item, "Customers", company.Id, tableName.Customer);

                return Ok(new ApiResponse<string> { Message = "Done" });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }
        [HttpGet("GetAllId")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllId()
        {

            try
            {
                var Company = await ClsCompanys.GetAllCustomersIdAndName();
                if (Company == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no Company" });
                return Ok(new ApiResponse<List<object>>
                {
                    Data = Company
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
        public async Task<ActionResult<ApiResponse<List<Co_Company_Name>>>> GetAll([FromQuery] int Page, [FromQuery] int count, [FromQuery]string? legals)
        {

            try
            {
                var Company = await ClsCompanys.GetAll(Page, count,legals);
                if (Company == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no Companys" });
                return Ok(new ApiResponse<List<Co_Company_Name>>
                {
                    Data = Company
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<Co_Company_Name>>
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
                var file = await ClsFiles.GetById(id, tableName.Company);
                if (file == null)
                    return NotFound(new ApiResponse<string> { Message = "the file is deleted or not found " });
                await ClsFiles.Delete(id, tableName.Company);

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"file {file.FileType} for {file.EntityId} in table{file.EntityType} Deleted from the System by {username} ", userId);
                // لهون هو تسجيل الlog  
                return Ok(new ApiResponse<string>
                {
                    Data = "done"
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
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                var Customer = await ClsCompanys.GetById(id);
                if (Customer == null)
                    return NotFound(new ApiResponse<string> { Message = "the field is deleted or not found " });
                await ClsCompanys.Delete(id);
                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"{Customer.Company.Name} Deleted from the System by {username} ", userId);
                // لهون هو تسجيل الlog  
                return Ok(new ApiResponse<string>
                {
                    Data = "done"
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<string>>
                {
                    Message = ex.Message,

                });
            }
        }
        [HttpGet("GetCount")]
        public async Task<ActionResult<ApiResponse<int>>> GetCount(string? filter)
        {
            try
            {
                var Companys = await ClsCompanys.Count(filter);
                if (Companys == 0 || Companys == null)
                {
                    return NotFound(new ApiResponse<int> { Message = "There is no Companys" });
                }

                return Ok(new ApiResponse<int>
                {
                    Data = Companys
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

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ApiResponse<CompanyModel>>> GetById(int id)
        {
            try
            {
                var company = await ClsCompanys.GetById(id);
                if (company == null)
                {
                    return NotFound(new ApiResponse<object> { Message = "company is deleted or could not found" });
                }

                return Ok(new ApiResponse<CompanyModel>
                {
                    Data = company
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
                var company = await ClsCompanys.Search(name, page, count);
                if (company == null)
                {
                    return NotFound(new ApiResponse<object> { Message = "No result" });
                }
                return Ok(new ApiResponse<object>
                {
                    Data = company
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
                var history = await ClsCompanys.LstEditHistory(page, id, count);
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