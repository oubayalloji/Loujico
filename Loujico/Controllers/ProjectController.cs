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
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddProjectModel dto, [FromForm] List<FileModel>? Data)
        {
            try { 
                if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
                        await ClsFiles.Add(item, "Projects", dto.Id, tableName.project);
                    }
                }
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

        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromBody] TbProject proj, [FromForm] List<FileModel>? Data)
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
                proj.UpdatedBy = username;
                await ClsProject.Edit(proj);
                // من هون 
                await ClsLogs.Add("Error", $"{proj.Title} updated to the System by {username} ", userId);
                // لهون هو تسجيل الlog
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Projects", proj.Id, tableName.project);
                        await ClsLogs.Add("CRUD", $"file {item.fileType} added to : {proj.Title} by {username} ", userId);

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
                return BadRequest(new ApiResponse<List<TbProject>>
                {
                    Message = ex.Message,

                });
            }


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
        [HttpDelete("DeleteImg/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteFile(int id)
        {
            try
            {
                var file = await ClsFiles.GetById(id, tableName.project);
                await ClsFiles.Delete(id, tableName.project);

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
        [HttpDelete("Delete")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                var project = await ClsProject.GetById(id);
                await ClsProject.Delete(id);

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("Error", $"{project.Title} Deleted from the System by {username} ", userId);
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
                var projectloyee = await ClsProject.GetById(id);

                return Ok(new ApiResponse<AddProjectModel>
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
