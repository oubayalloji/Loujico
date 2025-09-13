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
        ICustomers ClsCustomers;
        IEmployees ClsEmployee;
        Ilog ClsLogs;
        IProject ClsProjects;
        UserManager<ApplicationUser> UserManager;

        public DashbourdController( ICustomers clsCustomers, Ilog clsLogs, UserManager<ApplicationUser> userManager, IEmployees clsEmployee,IProject clsProject)
        {
          
             ClsCustomers= clsCustomers;
             ClsLogs=clsLogs;
            ClsProjects = clsProject;
           
            
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

                dashboard.Customer = await ClsCustomers.Count(); 
                dashboard.CountActiveUsers = await ClsEmployee.Count();
                dashboard.ActiveProjects = await ClsProjects.Count();
                dashboard.OverDueInvoices = await ClsCustomers.Count();
                var username = UserManager.GetUserName(User);
                dashboard.User = username;

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
