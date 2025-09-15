using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Loujico.Models;
using Loujico.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Loujico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

    public class ProjectController : ControllerBase
    {
        IFiles ClsFiles;
        IProject ClsProject;
        Ilog ClsLogs;
        IHistory ClsHistory;
        CompanySystemContext CTX;
        UserManager<ApplicationUser> UserManager;
        public ProjectController(IProject clsProject, CompanySystemContext context, UserManager<ApplicationUser> userManager, Ilog ilog, IHistory clsHistory, IFiles clsFiles)
        {
            ClsLogs = ilog;
            ClsProject = clsProject;
            CTX = context;
            UserManager = userManager;
            ClsHistory = clsHistory;
            ClsFiles = clsFiles;
            UserManager = userManager;
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromForm] AddProjectModel dto,[FromForm]  List<FileModel>? Data)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (dto.CustomerId == 0)
                {
                    return BadRequest("please enter customer id");
                }
                var username = UserManager.GetUserName(User);
                var project = new TbProject
                {
                    Title = dto.Title,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Price = dto.Price,
                    Progress = dto.Progress,
                    CreatedAt = DateTime.Now,
                    CreatedBy = username,
                    CustomerId = dto.CustomerId,
                };

                // ربط الموظفين بالمشروع
                foreach (var emp in dto.Employees)
                {
                    project.TbProjectsEmployees.Add(new TbProjectsEmployee
                    {
                        EmployeeId = emp.EmployeeId,
                        RoleOnProject = emp.RoleOnProject,
                        JoinedAt = DateTime.Now
                    });
                }

                CTX.TbProjects.Add(project);
                await CTX.SaveChangesAsync();
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Projects", project.Id, tableName.project);
                    }
                }
                var usename = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"{dto.Title} Added to the System by {usename} ", userId);

                return Ok(new { project.Id, message = "تمت إضافة المشروع بنجاح" });
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

        [HttpGet("GetAllId")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllId()
        {

            try
            {
                var Project = await ClsProject.GetAllProjectAndInvoice();
                if (Project == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no Projects" });
                return Ok(new ApiResponse<List<object>>
                {
                    Data = Project
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

        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit(
      [FromForm] AddProjectModel dto,
      [FromForm] List<FileModel>? Data)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Message = "Invalid payload." });

            // جلب المشروع مع علاقات الموظفين (المحذوفين يُستبعدون تلقائياً عبر HasQueryFilter)
            var proj = await CTX.TbProjects
                .Include(p => p.TbProjectsEmployees)
                .FirstOrDefaultAsync(p => p.Id == dto.Id && !p.IsDeleted);

            if (proj == null)
                return NotFound(new ApiResponse<string> { Message = "Project not found." });

            var username = UserManager.GetUserName(User);
            var userId = UserManager.GetUserId(User);

            // تحديث خصائص المشروع
            proj.Title = dto.Title;
            proj.StartDate = dto.StartDate;
            proj.EndDate = dto.EndDate;
            proj.Price = dto.Price;
            proj.Progress = dto.Progress;
            proj.CustomerId = dto.CustomerId;
            proj.UpdatedAt = DateTime.Now;
            proj.UpdatedBy = username;

            // 1. علّم جميع روابط الموظفين الحالية محذوفة
            foreach (var link in proj.TbProjectsEmployees)
            {
                link.IsDeleted = true;
            }

            // 2. عُد تفعيل أو أضف الروابط الواردة في dto.Employees
            foreach (var empDto in dto.Employees ?? Enumerable.Empty<EmployeeOnProjectModel>())
            {
                var match = proj.TbProjectsEmployees
                    .FirstOrDefault(pe =>
                        pe.EmployeeId == empDto.EmployeeId &&
                        pe.RoleOnProject == empDto.RoleOnProject);

                if (match != null)
                {
                    // إعادة التفعيل وتحديث وقت الانضمام
                    match.IsDeleted = false;
                    match.JoinedAt = DateTime.Now;
                }
                else
                {
                    // إضافة سجل جديد للموظف
                    proj.TbProjectsEmployees.Add(new TbProjectsEmployee
                    {
                        EmployeeId = empDto.EmployeeId,
                        RoleOnProject = empDto.RoleOnProject,
                        JoinedAt = DateTime.Now,
                        IsDeleted = false
                    });
                }
            }

            // حفظ التعديلات دفعة واحدة
            await CTX.SaveChangesAsync();

            // تسجيل السجلّات
            await ClsLogs.Add("CRUD", $"Project '{proj.Title}' updated by {username}.", userId);

            // معالجة الملفات إن وجدت
            if (Data != null)
            {
                foreach (var file in Data)
                {
                    await ClsFiles.Add(file, "Projects", proj.Id, tableName.project);
                    await ClsLogs.Add(
                        "CRUD",
                        $"File '{file.fileType}' added to project '{proj.Title}' by {username}.",
                        userId);
                }
            }

            return Ok(new ApiResponse<string> { Message = "Done" });
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAll([FromQuery] int Page, [FromQuery] int Count)
        {

            try
            {
                var fin = await ClsProject.Pagintion(Page,Count);

                return Ok(new ApiResponse<List<object>>
                {
                    Data = fin
                }) ;
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
        [HttpDelete("DeleteFile/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteFile(int id)
        {
            try
            {
                var file = await ClsFiles.GetById(id, tableName.project);
                if (file == null)
                    return NotFound("the file is deleted");
                await ClsFiles.Delete(id, tableName.project);

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
                var project = await ClsProject.GetById(id);
                await ClsProject.Delete(id);

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"{project.Title} Deleted from the System by {username} ", userId);
                // لهون هو تسجيل الlog  
                return Ok(new ApiResponse<String>
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

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ApiResponse<AddProjectModel>>> GetById(int id)
        {
            try
            {
                var projectloyee = await ClsProject.GetByIdModel(id);

                return Ok(new ApiResponse<ShowProject>
                {
                 
                    Data = projectloyee
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
        [HttpGet("GetCount")]
        public async Task<ActionResult<ApiResponse<int>>> Count()
        {
            try
            {
                var projectloyee = await ClsProject.Count();

                return Ok(new ApiResponse<int>
                {

                    Data = projectloyee
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
         [HttpGet("EditHistory")]
        public async Task<ActionResult<ApiResponse<List<TbHistory>>>> LstEditHistory([FromQuery]int page,[FromQuery] int id,[FromQuery] int count)
        {
            try
            {
                var history = await ClsProject.LstEditHistory(page, id, count);
                return Ok(new ApiResponse<List<TbHistory>> { Data = history });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbHistory>> { Message = ex.Message });
            }
        }




















































        [HttpGet("Search")]
        public async Task<ActionResult<ApiResponse<object>>> Search([FromQuery] string name, [FromQuery] int page, [FromQuery] int count)
        {
            try
            {
                var Project = await ClsProject.Search(name, page, count);
                if (Project == null)
                {
                    return NotFound(new ApiResponse<object> { Message = "No result" });
                }
                return Ok(new ApiResponse<object>
                {
                    Data = Project
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
    }
}
