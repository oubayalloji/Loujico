using Loujico.BL;
using Loujico.Migrations;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace Loujico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

    public class CompanyController : ControllerBase
    {
        CompanySystemContext CTX;
        //ICustomers ClsCustomers;
        Ilog ClsLogs;
        IHistory ClsHistory;
        UserManager<ApplicationUser> UserManager;
        IFiles ClsFiles;
        ICompanys ClsCompanys;
        
        public CompanyController(CompanySystemContext cTX, Ilog clsLogs, UserManager<ApplicationUser> userManager, IHistory clsHistory, IFiles clsFiles, ICompanys clsCompanys)
        {

            CTX = cTX;
     //       ClsCustomers = clsCustomers;
            ClsLogs = clsLogs;
            UserManager = userManager;
            ClsHistory = clsHistory;
            ClsFiles = clsFiles;
            ClsCompanys = clsCompanys;
        }
        [HttpPost("AddCompany")]
        public async Task<ActionResult<ApiResponse<CompanyReadDto>>> AddCompany([FromForm] CompanyCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Message = "Invalid model" });

            try
            {
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);

                // إذا أرسل المستخدم LegalId، تحقق من وجودها
                if (dto.LegalId.HasValue)
                {
                    var legalExists = await CTX.Co_Legals.AnyAsync(l => l.Id == dto.LegalId.Value);
                    if (!legalExists)
                        return BadRequest(new ApiResponse<string> { Message = $"Invalid LegalId: {dto.LegalId.Value}" });
                }

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
                    LegalId = dto.LegalId   // ربط الـ LegalId مباشرة
                };

                CTX.Co_Companies.Add(company);
                await CTX.SaveChangesAsync();

                await ClsLogs.Add("CRUD", $"{company.Name} added to the System by {username}", userId);

                // بناء رد مع بيانات الـ Legal إن وُجدت
                var read = new CompanyReadDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Comm_No = company.Comm_No,
                    Tax_No = company.Tax_No,
                    Found_Date = company.Found_Date,
                    CompanyDescription = company.CompanyDescription,
                    CreatedAt = company.CreatedAt,
                    CreatedBy = company.CreatedBy,
                    LegalId = company.LegalId
                };

                if (company.LegalId.HasValue)
                {
                    var legal = await CTX.Co_Legals
                                         .AsNoTracking()
                                         .Where(l => l.Id == company.LegalId.Value)
                                         .Select(l => new { l.Id, l.LegalInfo })
                                         .FirstOrDefaultAsync();

                    if (legal != null)
                        read.LegalInfo = legal.LegalInfo;
                }

                return Ok(new ApiResponse<CompanyReadDto> { Message = "Done", Data = read });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }

        [HttpPatch("EditCompany")]
        public async Task<ActionResult<ApiResponse<string>>> EditCompany([FromForm] CompanyUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Message = "Invalid model" });

            var username = UserManager.GetUserName(User);
            var userId = UserManager.GetUserId(User);

            var company = await CTX.Co_Companies.FirstOrDefaultAsync(c => c.Id == dto.id && !c.IsDeleted);
            if (company == null)
                return NotFound(new ApiResponse<string> { Message = "Not found" });

            // إذا أرسلت LegalId فنتحقق من وجوده قبل التغيير لتجنب FK error
            if (dto.LegalId.HasValue)
            {
                var legalExists = await CTX.Co_Legals.AnyAsync(l => l.Id == dto.LegalId.Value);
                if (!legalExists)
                    return BadRequest(new ApiResponse<string> { Message = $"Invalid LegalId: {dto.LegalId.Value}" });
            }

            try
            {
                // تحديث الحقول الأساسية
                company.Name = dto.Name;
                company.Comm_No = dto.Comm_No;
                company.Tax_No = dto.Tax_No;
                company.Found_Date = dto.Found_Date;
                company.CompanyDescription = dto.CompanyDescription;

                // ربط/إلغاء ربط الـ Legal بحسب قيمة الـ DTO
                company.LegalId = dto.LegalId; // يمكن أن تكون null => فك الربط

                company.UpdatedAt = DateTime.UtcNow;
                company.UpdatedBy = username;

                await CTX.SaveChangesAsync();
                await ClsLogs.Add("CRUD", $"{company.Name} updated by {username}", userId);

                return Ok(new ApiResponse<string> { Message = "Done" });
            }
            catch (DbUpdateException dbEx)
            {
                await ClsLogs.Add("Error", dbEx.Message, null);
                return BadRequest(new ApiResponse<string> { Message = "Database update error. Check provided FK ids." });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }




        // POST: api/companies/{companyId}/contacts
        [HttpPost("AddContacts/{companyId:int}")]
        public async Task<ActionResult<ApiResponse<List<CompanyContactReadDto>>>> AddContacts(int companyId, [FromBody] List<CompanyContactCreateDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new ApiResponse<string> { Message = "No contacts supplied" });

            await using var tx = await CTX.Database.BeginTransactionAsync();
            try
            {
                //     var companyIds = dtos.Select(d => companyId).Distinct().ToList();
                // optional: validate single companyId or existence of company(ies) / contact types here

                var created = new List<CompanyContactReadDto>();
                foreach (var dto in dtos)
                {
                    if (!ModelState.IsValid) // أو تحقق لكل dto يدوياً
                        return BadRequest(new ApiResponse<string> { Message = "Invalid contact data" });

                    var contact = new Co_Contact
                    {
                        CompanyId = companyId,        // إذا DTO لا يحمل CompanyId, استخدم مسار api/companies/{id}/contacts بدلًا من قائمة عامة
                        ContactTypeId = dto.ContactTypeId,
                        Name = dto.Name
                    };

                    CTX.Co_Contacts.Add(contact);
                    created.Add(new CompanyContactReadDto
                    {
                        Id = 0, // سيُملأ بعد SaveChanges
                        CompanyId = contact.CompanyId,
                        ContactTypeId = contact.ContactTypeId,
                        ContactTypeName = "", // يمكن ملأها بعد الحفظ أو بعمل join
                        Name = contact.Name
                    });
                }

                await CTX.SaveChangesAsync();

                // الآن عبيّن الـ Ids و contactTypeName إن أردت
                for (int i = 0; i < created.Count; i++)
                {
                    created[i].Id = CTX.Co_Contacts.OrderByDescending(c => c.Id).Skip(i).Select(c => c.Id).FirstOrDefault();
                }

                await tx.CommitAsync();
                return Ok(new ApiResponse<List<CompanyContactReadDto>> { Message = "Done", Data = created });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }

        [HttpPatch("EditContacts/{companyId:int}")]
        public async Task<ActionResult<ApiResponse<List<CompanyContactReadDto>>>> EditContacts(int companyId, [FromBody] List<CompanyContactCreateDto> dtos)
        {
            if (dtos == null) 
                return BadRequest(new ApiResponse<string> { Message = "Payload is required" });

            await using var tx = await CTX.Database.BeginTransactionAsync();
            try
            {
                var company = await CTX.Co_Companies.FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted);
                if (company == null) return NotFound(new ApiResponse<string> { Message = "Company not found" });

                // اختياري: تحقق من وجود ContactTypeIds المقدمة (مفضل لتجنب FK exceptions)
                var distinctTypeIds = dtos.Select(d => d.ContactTypeId).Distinct().ToList();
                var validTypeIds = await CTX.TbContact.Where(t => distinctTypeIds.Contains(t.Id)).Select(t => t.Id).ToListAsync();
                var invalid = distinctTypeIds.Except(validTypeIds).ToList();
                if (invalid.Any())
                    return BadRequest(new ApiResponse<string> { Message = $"Invalid ContactTypeIds: {string.Join(',', invalid)}" });

                // حذف كل جهات الاتصال الحالية للشركة
                var oldContacts = await CTX.Co_Contacts.Where(c => c.CompanyId == companyId).ToListAsync();
                if (oldContacts.Any())
                    CTX.Co_Contacts.RemoveRange(oldContacts);

                // إضافة الجديدة
                var toAdd = dtos.Select(d => new Co_Contact
                {
                    CompanyId = companyId,
                    ContactTypeId = d.ContactTypeId,
                    Name = d.Name
                }).ToList();

                if (toAdd.Any())
                    await CTX.Co_Contacts.AddRangeAsync(toAdd);

                await CTX.SaveChangesAsync();
                await tx.CommitAsync();

                // بناء قائمة الــ Read DTOs مع أسماء أنواع الاتصال
                var added = await CTX.Co_Contacts
                    .AsNoTracking()
                    .Where(c => c.CompanyId == companyId)
                    .Include(c => c.ContactType)
                    .Select(c => new CompanyContactReadDto
                    {
                        Id = c.Id,
                        CompanyId = c.CompanyId,
                        ContactTypeId = c.ContactTypeId,
                        ContactTypeName = c.ContactType != null ? c.ContactType.Name : "",
                        Name = c.Name
                    })
                    .ToListAsync();

                await ClsLogs.Add("CRUD", $"Contacts replaced for CompanyId {companyId} by {UserManager.GetUserId(User)}", UserManager.GetUserId(User));

                return Ok(new ApiResponse<List<CompanyContactReadDto>> { Message = "Done", Data = added });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }




        [HttpPost("AddActivities/{companyId:int}")]
        public async Task<ActionResult<ApiResponse<List<CompanyActivityReadDto>>>> AddActivities(int companyId, [FromBody] List<CompanyActivityLinkDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new ApiResponse<string> { Message = "No activity ids supplied" });

            var companyExists = await CTX.Co_Companies.AnyAsync(c => c.Id == companyId && !c.IsDeleted);
            if (!companyExists) return NotFound(new ApiResponse<string> { Message = "Company not found" });

            var activityIds = dtos.Select(d => d.activityId).Distinct().ToList();

            // جلب الـ activities مع IndustryId واسم Industry للتحقق والتعبئة
            var activities = await CTX.Co_Activities
                                      .Where(a => activityIds.Contains(a.Id))
                                      .Select(a => new { a.Id, a.Name, a.IndustryId, IndustryName = a.Industry != null ? a.Industry.Name : string.Empty })
                                      .ToListAsync();

            var validActivityIds = activities.Select(a => a.Id).ToList();
            var invalid = activityIds.Except(validActivityIds).ToList();
            if (invalid.Any())
                return BadRequest(new ApiResponse<string> { Message = $"Invalid activity ids: {string.Join(',', invalid)}" });

            // existing links to avoid duplicates
            var existingLinks = await CTX.Co_CompanyActivities
                                         .Where(x => x.CompanyId == companyId && activityIds.Contains(x.ActivityId))
                                         .Select(x => x.ActivityId)
                                         .ToListAsync();

            var toAdd = activities
                        .Where(a => !existingLinks.Contains(a.Id))
                        .Select(a => new CompanyActivity
                        {
                            CompanyId = companyId,
                            ActivityId = a.Id,
                            IndustryId = a.IndustryId
                        })
                        .ToList();

            if (!toAdd.Any())
                return Ok(new ApiResponse<string> { Message = "No new activities to add" });

            await CTX.Co_CompanyActivities.AddRangeAsync(toAdd);
            await CTX.SaveChangesAsync();

            // إحضار الروابط الحالية بعد الإضافة مع أسماء النشاط والصناعة
            var added = await CTX.Co_CompanyActivities
                                 .AsNoTracking()
                                 .Where(x => x.CompanyId == companyId && activityIds.Contains(x.ActivityId))
                                 .Include(x => x.Activity)
                                 .ThenInclude(a => a.Industry)
                                 .Select(x => new CompanyActivityReadDto
                                 {
                                     Id = x.Id,
                                     CompanyId = x.CompanyId,
                                     ActivityId = x.ActivityId,
                                     ActivityName = x.Activity != null ? x.Activity.Name : string.Empty,
                                     IndustryId = x.IndustryId,
                                     IndustryName = x.Activity != null && x.Activity.Industry != null ? x.Activity.Industry.Name : string.Empty
                                 })
                                 .ToListAsync();

            await ClsLogs.Add("CRUD", $"Added activities to CompanyId {companyId}", UserManager.GetUserId(User));
            return Ok(new ApiResponse<List<CompanyActivityReadDto>> { Message = "Done", Data = added });
        }

        [HttpPatch("EditActivities/{companyId:int}")]
        public async Task<ActionResult<ApiResponse<List<CompanyActivityReadDto>>>> EditActivities(
      int companyId, [FromBody] List<CompanyActivityLinkDto> dtos)
        {
            if (dtos == null)
                return BadRequest(new ApiResponse<string> { Message = "Payload is required" });

            // جمع الـ ids المرسلة والتحقق من القيم الإيجابية
            var distinctIds = dtos.Select(d => d.activityId).Where(id => id > 0).Distinct().ToList();

            await using var tx = await CTX.Database.BeginTransactionAsync();
            try
            {
                var company = await CTX.Co_Companies.FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted);
                if (company == null) return NotFound(new ApiResponse<string> { Message = "Company not found" });

                // حالة المسح الكامل إذا القائمة فارغة
                if (!distinctIds.Any())
                {
                    var oldAll = await CTX.Co_CompanyActivities.Where(x => x.CompanyId == companyId).ToListAsync();
                    if (oldAll.Any()) CTX.Co_CompanyActivities.RemoveRange(oldAll);

                    await CTX.SaveChangesAsync();
                    await tx.CommitAsync();

                    await ClsLogs.Add("CRUD", $"Activities replaced for CompanyId {companyId} (cleared all)", UserManager.GetUserId(User));
                   return Ok(new ApiResponse<List<CompanyActivityReadDto>> { Message = "Done", Data = new List<CompanyActivityReadDto>() });
                }

                // تحقق من وجود الأنشطة المرسلة في الجدول لتجنب FK errors
                var validActivities = await CTX.Co_Activities
                    .Where(a => distinctIds.Contains(a.Id))
                    .Select(a => new { a.Id, a.Name, a.IndustryId, IndustryName = a.Industry != null ? a.Industry.Name : string.Empty })
                    .ToListAsync();

                var foundIds = validActivities.Select(a => a.Id).ToList();
                var invalid = distinctIds.Except(foundIds).ToList();
                if (invalid.Any())
                    return BadRequest(new ApiResponse<string> { Message = $"Invalid activity ids: {string.Join(',', invalid)}" });

                // حذف كل الروابط القديمة للشركة
                var oldLinks = await CTX.Co_CompanyActivities.Where(x => x.CompanyId == companyId).ToListAsync();
                if (oldLinks.Any()) CTX.Co_CompanyActivities.RemoveRange(oldLinks);

                // إضافة الروابط الجديدة مع تعبئة IndustryId من validActivities
                var newLinks = validActivities.Select(a => new CompanyActivity
                {
                    CompanyId = companyId,
                    ActivityId = a.Id,
                    IndustryId = a.IndustryId
                }).ToList();

                if (newLinks.Any()) await CTX.Co_CompanyActivities.AddRangeAsync(newLinks);

                await CTX.SaveChangesAsync();
                await tx.CommitAsync();

                // جلب النتيجة النهائية مع أسماء النشاط والصناعة
                var added = await CTX.Co_CompanyActivities
                    .AsNoTracking()
                    .Where(x => x.CompanyId == companyId)
                    .Include(x => x.Activity)
                    .ThenInclude(a => a.Industry)
                    .Select(x => new CompanyActivityReadDto
                    {
                        Id = x.Id,
                        CompanyId = x.CompanyId,
                        ActivityId = x.ActivityId,
                        ActivityName = x.Activity != null ? x.Activity.Name : string.Empty,
                        IndustryId = x.IndustryId,
                        IndustryName = x.Activity != null && x.Activity.Industry != null ? x.Activity.Industry.Name : string.Empty
                    })
                    .ToListAsync();

                await ClsLogs.Add("CRUD", $"Activities replaced for CompanyId {companyId} by {UserManager.GetUserId(User)}", UserManager.GetUserId(User));
                return Ok(new ApiResponse<List<CompanyActivityReadDto>> { Message = "Done", Data = added });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }


        [HttpPost("AddFiles/{companyId:int}")]
        public async Task<ActionResult<ApiResponse<string>>> AddCompanyFiles(int companyId, [FromForm] List<FileModel>? Data)
        {
            if (Data == null || !Data.Any())
                return BadRequest(new ApiResponse<string> { Message = "No files supplied" });

            var company = await CTX.Co_Companies.FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted);
            if (company == null)
                return NotFound(new ApiResponse<string> { Message = "Company not found" });

            var username = UserManager.GetUserName(User);
            var userId = UserManager.GetUserId(User);

            await using var tx = await CTX.Database.BeginTransactionAsync();
            try
            {
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Companys", companyId, tableName.Company);
                    }
                }
                await CTX.SaveChangesAsync();
                await tx.CommitAsync();

                await ClsLogs.Add("Files", $"Added {Data.Count(f => f?.Files != null)} files for Company {company.Id} by {username}", userId);
                return Ok(new ApiResponse<string> { Message = "Files uploaded" });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }




        [HttpPost("AddEmployees/{companyId:int}")]
        public async Task<ActionResult<ApiResponse<List<CompanyEmployeeReadDto>>>> AddEmployees(
            int companyId, [FromBody] List<CompanyEmployeeCreateDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new ApiResponse<string> { Message = "No employees supplied" });

            var company = await CTX.Co_Companies.FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted);
            if (company == null)
                return NotFound(new ApiResponse<string> { Message = "Company not found" });

            // تحقق من ContactTypeIds الموجودين في كل الـ DTOs لتجنب FK exceptions
            var contactTypeIds = dtos
                .Where(d => d.Contacts != null)
                .SelectMany(d => d.Contacts!)
                .Select(c => c.ContactTypeId)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (contactTypeIds.Any())
            {
                var validTypes = await CTX.TbContact
                    .Where(t => contactTypeIds.Contains(t.Id))
                    .Select(t => t.Id)
                    .ToListAsync();

                var invalid = contactTypeIds.Except(validTypes).ToList();
                if (invalid.Any())
                    return BadRequest(new ApiResponse<string> { Message = $"Invalid ContactTypeIds: {string.Join(',', invalid)}" });
            }

            await using var tx = await CTX.Database.BeginTransactionAsync();
            try
            {
                // 1) أنشئ الموظفين أولاً للحصول على الـ Ids
                var employees = dtos.Select(d => new Co_CompanyEmployee
                {
                    CompanyId = companyId,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Position = d.Position,
                    Department = d.Department,
                    Notes = d.Notes
                }).ToList();

                if (employees.Any())
                    await CTX.Co_CompanyEmployees.AddRangeAsync(employees);

                await CTX.SaveChangesAsync(); // نحصل على Ids

                // 2) أنشئ Contacts المرتبطة بكل موظف (إذا وُجدت)
                var contactsToAdd = new List<Co_Contact>();
                for (int i = 0; i < dtos.Count; i++)
                {
                    var dto = dtos[i];
                    var emp = employees.ElementAtOrDefault(i);
                    if (emp == null) continue;

                    if (dto.Contacts != null && dto.Contacts.Any())
                    {
                        foreach (var cDto in dto.Contacts)
                        {
                            if (string.IsNullOrWhiteSpace(cDto.Name) || cDto.ContactTypeId <= 0) continue;

                            contactsToAdd.Add(new Co_Contact
                            {
                                CompanyId = companyId,
                                EmployeeId = emp.Id,
                                ContactTypeId = cDto.ContactTypeId,
                                Name = cDto.Name
                            });
                        }
                    }
                }

                if (contactsToAdd.Any())
                {
                    await CTX.Co_Contacts.AddRangeAsync(contactsToAdd);
                    await CTX.SaveChangesAsync();
                }

                await tx.CommitAsync();

                // 3) إرجاع النتيجة: الموظفين مع قائمة Contacts لكل واحد (بأسماء أنواع الاتصال)
               

                await ClsLogs.Add("CRUD", $"Added {dtos.Count} employees (with contacts) to Company {companyId}", UserManager.GetUserId(User));

                return Ok(new ApiResponse<List<CompanyEmployeeReadDto>> { Message = "Done" });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }


        [HttpPatch("EditEmployees/{companyId:int}")]
        public async Task<ActionResult<ApiResponse<List<CompanyEmployeeReadDto>>>> EditEmployees(
    int companyId, [FromBody] List<CompanyEmployeeUpdateDto> dtos)
        {
            if (dtos == null )
                return BadRequest(new ApiResponse<string> { Message = "Payload is required" });

            await using var tx = await CTX.Database.BeginTransactionAsync();
            try
            {
                var company = await CTX.Co_Companies
                    .FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted);
                if (company == null)
                    return NotFound(new ApiResponse<string> { Message = "Company not found" });

                // validate contact types provided across all contacts
                var contactTypeIds = dtos
                    .Where(d => d.Contacts != null)
                    .SelectMany(d => d.Contacts!)
                    .Select(c => c.ContactTypeId)
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();

                if (contactTypeIds.Any())
                {
                    var validTypes = await CTX.TbContact
                        .Where(t => contactTypeIds.Contains(t.Id))
                        .Select(t => t.Id)
                        .ToListAsync();

                    var invalidTypes = contactTypeIds.Except(validTypes).ToList();
                    if (invalidTypes.Any())
                        return BadRequest(new ApiResponse<string> { Message = $"Invalid ContactTypeIds: {string.Join(',', invalidTypes)}" });
                }

                // 1) حذف كل الموظفين الحاليين للشركة
                var oldEmployees = await CTX.Co_CompanyEmployees
                    .Where(e => e.CompanyId == companyId)
                    .ToListAsync();
                if (oldEmployees.Any())
                    CTX.Co_CompanyEmployees.RemoveRange(oldEmployees);

                // 2) حذف كل وسائل الاتصال المرتبطة بموظفين هذه الشركة (EmployeeId != null)
                var oldContacts = await CTX.Co_Contacts
                    .Where(c => c.CompanyId == companyId && c.EmployeeId != null)
                    .ToListAsync();
                if (oldContacts.Any())
                    CTX.Co_Contacts.RemoveRange(oldContacts);

                await CTX.SaveChangesAsync();

                // 3) إنشاء الموظفين الجدد (بدون Contacts) حتى نحصل على Ids
                var employees = dtos.Select(d => new Co_CompanyEmployee
                {
                    CompanyId = companyId,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Position = d.Position,
                    Department = d.Department,
                    Notes = d.Notes
                }).ToList();

                if (employees.Any())
                    await CTX.Co_CompanyEmployees.AddRangeAsync(employees);

                await CTX.SaveChangesAsync(); // نحصل على Ids للموظفين

                // 4) بناء وإضافة وسائل الاتصال المرتبطة بكل موظف
                var contactsToAdd = new List<Co_Contact>();
                for (int i = 0; i < dtos.Count; i++)
                {
                    var dto = dtos[i];
                    var emp = employees.ElementAtOrDefault(i);
                    if (emp == null) continue;

                    if (dto.Contacts != null && dto.Contacts.Any())
                    {
                        foreach (var cDto in dto.Contacts)
                        {
                            if (string.IsNullOrWhiteSpace(cDto.Name) || cDto.ContactTypeId <= 0) continue;

                            contactsToAdd.Add(new Co_Contact
                            {
                                CompanyId = companyId,
                                EmployeeId = emp.Id,
                                ContactTypeId = cDto.ContactTypeId,
                                Name = cDto.Name
                            });
                        }
                    }
                }

                if (contactsToAdd.Any())
                {
                    await CTX.Co_Contacts.AddRangeAsync(contactsToAdd);
                    await CTX.SaveChangesAsync();
                }

                await tx.CommitAsync();

                // 5) بناء نتيجة القراءة: الموظف مع وسائل الاتصال (مع أسماء أنواع الاتصال)
             

                await ClsLogs.Add("CRUD", $"Employees replaced (with contacts) for CompanyId {companyId}", UserManager.GetUserId(User));

                return Ok(new ApiResponse<List<CompanyEmployeeReadDto>> { Message = "Done" });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }

        /* [HttpPost("AddAddress/{companyId:int}")]
         public async Task<ActionResult<ApiResponse<CompanyAddressReadDto>>> AddAddress(int companyId, [FromBody] CompanyAddressCreateDto dto)
         {
             if (dto == null) return BadRequest(new ApiResponse<string> { Message = "Payload required" });

             var company = await CTX.Co_Companies.FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted);
             if (company == null) return NotFound(new ApiResponse<string> { Message = "Company not found" });

             // اختياري: تحقق من وجود Country/State/City
             var valid = await CTX.TbCountries.AnyAsync(x => x.Id == dto.CountryId)
                      && await CTX.TbStates.AnyAsync(x => x.Id == dto.StateId)
                      && await CTX.TbCities.AnyAsync(x => x.Id == dto.CityId);
             if (!valid) return BadRequest(new ApiResponse<string> { Message = "Invalid location ids" });

             var addr = new Co_Address
             {
                 CompanyId = companyId,
                 CountryId = dto.CountryId,
                 StateId = dto.StateId,
                 CityId = dto.CityId,
                 AddressLine = dto.AddressLine
             };

             CTX.Co_Address.Add(addr);
             await CTX.SaveChangesAsync();


             return Ok(new ApiResponse<CompanyAddressReadDto> { Message = "Done"});
         }*/
        [HttpPost("AddAddresses/{companyId:int}")]
        public async Task<ActionResult<ApiResponse<List<CompanyAddressReadDto>>>> AddAddresses(
     int companyId, [FromBody] List<CompanyAddressCreateDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new ApiResponse<string> { Message = "No addresses supplied" });

            var company = await CTX.Co_Companies.FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted);
            if (company == null)
                return NotFound(new ApiResponse<string> { Message = "Company not found" });

            // اختياري: تحقق من صحة Country/State/City ids لجميع الـ DTOs لتجنب FK exceptions
            var countryIds = dtos.Select(d => d.CountryId).Distinct().ToList();
            var stateIds = dtos.Select(d => d.StateId).Distinct().ToList();
            var cityIds = dtos.Select(d => d.CityId).Distinct().ToList();

            var validCountries = await CTX.TbCountries.Where(c => countryIds.Contains(c.Id)).Select(c => c.Id).ToListAsync();
            var validStates = await CTX.TbStates.Where(s => stateIds.Contains(s.Id)).Select(s => s.Id).ToListAsync();
            var validCities = await CTX.TbCities.Where(c => cityIds.Contains(c.Id)).Select(c => c.Id).ToListAsync();

            var invalidCountry = countryIds.Except(validCountries).ToList();
            var invalidState = stateIds.Except(validStates).ToList();
            var invalidCity = cityIds.Except(validCities).ToList();

            if (invalidCountry.Any() || invalidState.Any() || invalidCity.Any())
                return BadRequest(new ApiResponse<string>
                {
                    Message = $"Invalid ids - Countries: [{string.Join(',', invalidCountry)}], States: [{string.Join(',', invalidState)}], Cities: [{string.Join(',', invalidCity)}]"
                });

            await using var tx = await CTX.Database.BeginTransactionAsync();
            try
            {
             
                foreach (var dto in dtos)
                {
                    if (!ModelState.IsValid)
                        return BadRequest(new ApiResponse<string> { Message = "Invalid address data" });

                    var addr = new Co_Address
                    {
                        CompanyId = companyId,
                        CountryId = dto.CountryId,
                        StateId = dto.StateId,
                        CityId = dto.CityId,
                        AddressLine = dto.AddressLine
                    };

                    CTX.Co_Address.Add(addr);

                  
                }

                await CTX.SaveChangesAsync();

 

       

                await tx.CommitAsync();
                return Ok(new ApiResponse<List<CompanyAddressReadDto>> { Message = "Done" });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }
        [HttpPatch("EditAddresses/{companyId:int}")]
        public async Task<ActionResult<ApiResponse<List<CompanyAddressReadDto>>>> EditAddresses(
      int companyId, [FromBody] List<CompanyAddressCreateDto> dtos)
        {
            if (dtos == null)
                return BadRequest(new ApiResponse<string> { Message = "Payload required" });

            var company = await CTX.Co_Companies.FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted);
            if (company == null)
                return NotFound(new ApiResponse<string> { Message = "Company not found" });

            // تحقق مسبق من صلاحية Country/State/City ids (اختياري لكنه موصى به)
            var countryIds = dtos.Select(d => d.CountryId).Distinct().ToList();
            var stateIds = dtos.Select(d => d.StateId).Distinct().ToList();
            var cityIds = dtos.Select(d => d.CityId).Distinct().ToList();

            var validCountries = await CTX.TbCountries.Where(c => countryIds.Contains(c.Id)).Select(c => c.Id).ToListAsync();
            var validStates = await CTX.TbStates.Where(s => stateIds.Contains(s.Id)).Select(s => s.Id).ToListAsync();
            var validCities = await CTX.TbCities.Where(c => cityIds.Contains(c.Id)).Select(c => c.Id).ToListAsync();

            var invalidCountry = countryIds.Except(validCountries).ToList();
            var invalidState = stateIds.Except(validStates).ToList();
            var invalidCity = cityIds.Except(validCities).ToList();

            if (invalidCountry.Any() || invalidState.Any() || invalidCity.Any())
                return BadRequest(new ApiResponse<string>
                {
                    Message = $"Invalid ids - Countries: [{string.Join(',', invalidCountry)}], States: [{string.Join(',', invalidState)}], Cities: [{string.Join(',', invalidCity)}]"
                });

            await using var tx = await CTX.Database.BeginTransactionAsync();
            try
            {
                // حذف كل العناوين القديمة للشركة
                var old = await CTX.Co_Address.Where(a => a.CompanyId == companyId).ToListAsync();
                if (old.Any())
                    CTX.Co_Address.RemoveRange(old);

                await CTX.SaveChangesAsync();

                // تحضير وإضافة العناوين الجديدة دفعة واحدة
                var toAdd = dtos.Select(d => new Co_Address
                {
                    CompanyId = companyId,
                    CountryId = d.CountryId,
                    StateId = d.StateId,
                    CityId = d.CityId,
                    AddressLine = d.AddressLine
                }).ToList();

                if (toAdd.Any())
                    await CTX.Co_Address.AddRangeAsync(toAdd);

                var saved = await CTX.SaveChangesAsync();
                await tx.CommitAsync();

                // جلب النتيجة النهائية مع أسماء Country/State/City
           

                await ClsLogs.Add("CRUD", $"Addresses replaced for CompanyId {companyId} by {UserManager.GetUserId(User)}", UserManager.GetUserId(User));
                return Ok(new ApiResponse<List<CompanyAddressReadDto>> { Message = "Done" });
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
        public async Task<ActionResult<ApiResponse<List<Co_Company_Name>>>> GetAll([FromQuery] int Page, [FromQuery] int count, [FromQuery]int? legals)
        {

            try
            {
                var Company = await ClsCompanys.GetAll(Page, count,legals);
                if (Company == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no Companys" });
                return Ok(new 
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
                if (!await ClsFiles.Delete(id, tableName.Company))
                {
                    return NotFound(new ApiResponse<string> { Message = "the file can not be deleted" });

                }


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
                if (!await ClsCompanys.Delete(id))
                {
                    return NotFound(new ApiResponse<string> { Message = "the company can not be deleted" });

                };
                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $" Deleted from the System by {username} ", userId);
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
                    return NotFound(new ApiResponse<int> { Message = "There is no Companys" , Data = 0 });
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

                return Ok(new ApiResponse<object>
                {
                    Data = company
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
                return BadRequest(new ApiResponse<List<string>>
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