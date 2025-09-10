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
        IProject ClsProject;
        Ilog ClsLogs;
        IHistory ClsHistory;
        CompanySystemContext CTX;
        UserManager<ApplicationUser> UserManager;
        public ProjectController(IProject clsProject, CompanySystemContext context, UserManager<ApplicationUser> userManager, Ilog ilog, IHistory clsHistory)
        {
            ClsLogs = ilog;
            ClsProject = clsProject;
            CTX = context;
            UserManager = userManager;
            ClsHistory = clsHistory;

        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddProjectModel dto)
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
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromForm] TbProject proj)
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
    }
}
