using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Loujico.Models;
using Loujico.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Loujico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        IProject ClsProject;
        CompanySystemContext CTX;
        UserManager<ApplicationUser> UserManager;
        public ProjectController(IProject clsProject, CompanySystemContext context, UserManager<ApplicationUser> _UserManager)
        {

            ClsProject = clsProject;
            CTX = context;
            UserManager = _UserManager;


        }
        [HttpPost("Add")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromForm] TbProject proj)
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
             if(await ClsProject.Add(proj))
                {
                    return Ok(new ApiResponse<String>
                    {
                    
                        Message = "project added successfully"

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
                return StatusCode(500,ex.Message);
            }
        }
        public ActionResult<ApiResponse<TbProject>> Show(int id)
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
    }
}
