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

    public class DashbourdController : ControllerBase
    {
        CompanySystemContext CTX;
        ICustomers ClsCustomers;
        IEmployees ClsEmployee;
        Ilog ClsLogs;
        IHistory ClsHistory;
        IProject ClsProjects;
        UserManager<ApplicationUser> UserManager;
        IFiles ClsFiles;
        public DashbourdController(CompanySystemContext cTX, ICustomers clsCustomers, Ilog clsLogs, UserManager<ApplicationUser> userManager, IHistory clsHistory, IFiles clsFiles, IEmployees clsEmployee,IProject clsProject)
        {
            CompanySystemContext CTX;
             ClsCustomers= clsCustomers;
             ClsLogs=clsLogs;
            ClsProjects = clsProject;
           
            
            UserManager<ApplicationUser> UserManager;
            IFiles ClsFiles;
            ClsEmployee = clsEmployee;

        }
        [HttpGet("GetDashboard")]
        public async Task<ActionResult<ApiResponse<object>>> GetDashboard()
        {

            try
            {
                DashboardModel dashboard = new DashboardModel();

                dashboard.Customer = await ClsCustomers.Count(); 
                dashboard.CountActiveUsers = await ClsEmployee.Count();
                dashboard.ActiveProjects = await ClsProjects.Count();
                dashboard.OverDueInvoices = await ClsCustomers.Count();
                var users = UserManager.GetUserName(User);

                if (dashboard == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no Customers" });
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
