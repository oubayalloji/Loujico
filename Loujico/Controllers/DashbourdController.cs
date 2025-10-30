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


    public class DashbourdController : ControllerBase
    {
        ICompanys ClsCompany;
        IEmployees ClsEmployee;
        Ilog ClsLogs;
        IProject ClsProjects;
       // IInvoices ClsInvoices;
        UserManager<ApplicationUser> UserManager;

        public DashbourdController(  Ilog clsLogs, UserManager<ApplicationUser> userManager, IEmployees clsEmployee,IProject clsProject,ICompanys companys)
        {

            ClsCompany = companys;
             ClsLogs=clsLogs;
            ClsProjects = clsProject;
           
            //ClsInvoices= invoices;
           UserManager=  userManager;
    
            ClsEmployee = clsEmployee;

        }
        [HttpGet("GetDashboard")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> GetDashboard()
        {

            try
            {
                DashboardModel dashboard = new DashboardModel();

                dashboard.Customer = await ClsCompany.Count(null); 
                dashboard.CountActiveEmployee = await ClsEmployee.Count();
                dashboard.ActiveProjects = await ClsProjects.CountPending();
              //  dashboard.OverDueInvoices = await ClsInvoices.CountOverdue();
                var username = UserManager.GetUserName(User);
                dashboard.User = username;

                if (dashboard == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no data" });
                return Ok(new ApiResponse<DashboardModel>
                {
                    Data = dashboard
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
    }
}
