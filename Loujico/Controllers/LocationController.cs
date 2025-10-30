using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace Loujico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

    public class LocationController : ControllerBase
    {
        Isettings ClsSettings;
        Ilog ClsLogs;
        IHistory ClsHistory;
        IFiles ClsFiles;

        CompanySystemContext CTX;
        UserManager<ApplicationUser> UserManager;
        public LocationController(Isettings clssettings, Ilog ilog, IHistory ihistory, IFiles files, UserManager<ApplicationUser> userManager, CompanySystemContext context)
        {
            ClsSettings = clssettings;
            ClsLogs = ilog;
            ClsHistory = ihistory;
            CTX = context;
            UserManager = userManager;
        }
        [HttpGet("GetAllCity")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllCity()
        {

            try
            {
                var Customer = await ClsSettings.GetAllCityType();
                if (Customer == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no Cities" });
                return Ok(new ApiResponse<List<object>>
                {
                    Data = Customer
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
        [HttpGet("GetAllState")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllState()
        {

            try
            {
                var State = await ClsSettings.GetAllStateType();
                if (State == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no States" });
                return Ok(new ApiResponse<List<object>>
                {
                    Data = State
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
        [HttpGet("GetAllCountry")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllCountry()
        {

            try
            {
                var Country = await ClsSettings.GetAllCountryType();
                if (Country == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no Countries" });
                return Ok(new ApiResponse<List<object>>
                {
                    Data = Country
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
        [HttpGet("GetCityByState/{StateId}")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetCityByState(int StateId)
        {

            try
            {
                var result = await ClsSettings.GetCityByState(StateId);
                if (result == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no result" });
                return Ok(new ApiResponse<List<object>>
                {
                    Data = result
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
        [HttpGet("GetStateByCountry/{CountryId}")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetStateByCountry(int CountryId)
        {

            try
            {
                var result = await ClsSettings.GetStateByCountry(CountryId);
                if (result == null)
                    return NotFound(new ApiResponse<string> { Message = "There is no result" });
                return Ok(new ApiResponse<List<object>>
                {
                    Data = result
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



        [HttpPost("AddCity/{StateId}")]
        public async Task<ActionResult<ApiResponse<string>>> AddCity([FromForm] string Name,int StateId)
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
                TbCity City = new TbCity();
                City.Name = Name;
                City.StateId = StateId;
                if (!await ClsSettings.AddCity(City))
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Message = "the City can not be added "
                    });
                }

                // من هون 
                await ClsLogs.Add("CRUD", $"{City.Name} added to the System by {username} ", userId);
                return Ok(new ApiResponse<String>
                {

                    Message = "Done"

                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });
            }


        }
        [HttpPost("AddCountry")]
        public async Task<ActionResult<ApiResponse<string>>> AddCountry([FromForm] string Name)
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
                TbCountry Country = new TbCountry();
                Country.Name = Name;
                if (!await ClsSettings.AddCountry(Country))
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Message = "the Country can not be added "
                    });
                }

                // من هون 
                await ClsLogs.Add("CRUD", $"{Country.Name} added to the System by {username} ", userId);
                return Ok(new ApiResponse<String>
                {

                    Message = "Done"

                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });
            }


        }  
        [HttpPost("AddState/{CountryId}")]
        public async Task<ActionResult<ApiResponse<string>>> AddState([FromForm] string Name, int CountryId)
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
                TbState State = new TbState();
                State.Name = Name;
                State.CountrId = CountryId;
                if (!await ClsSettings.AddState(State))
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Message = "the State can not be added "
                    });
                }

                // من هون 
                await ClsLogs.Add("CRUD", $"{State.Name} added to the System by {username} ", userId);
                return Ok(new ApiResponse<String>
                {

                    Message = "Done"

                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });
            }


        }



        [HttpDelete("DeleteState/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteState(int id)
        {
            try
            {
                var State = CTX.TbStates.FirstOrDefault(x => x.Id == id);
                var del = await ClsSettings.DeleteState(id);
                if (del == false)
                {
                    return NotFound(new ApiResponse<List<int>>
                    {
                        Message = "the state not found ",
                    });
                }

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"{State.Name} Deleted from the System by {username} ", userId);
                // لهون هو تسجيل الlog  
                return Ok(new ApiResponse<string>
                {

                    Data = "done"
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });
            }
        }

        [HttpDelete("DeleteCountry/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCountry(int id)
        {
            try
            {
                var Country = CTX.TbCountries.FirstOrDefault(x => x.Id == id);
                var del = await ClsSettings.DeleteCountry(id);
                if (del == false)
                {
                    return NotFound(new ApiResponse<List<int>>
                    {
                        Message = "the Country not found ",
                    });
                }

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"{Country.Name} Deleted from the System by {username} ", userId);
                // لهون هو تسجيل الlog  
                return Ok(new ApiResponse<string>
                {

                    Data = "done"
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });
            }
        }

        [HttpDelete("DeleteCity/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCity(int id)
        {
            try
            {
                var City = CTX.TbCities.FirstOrDefault(x => x.Id == id);
                var del = await ClsSettings.DeleteCity(id);
                if (del == false)
                {
                    return NotFound(new ApiResponse<List<int>>
                    {
                        Message = "the City not found ",
                    });
                }

                // من هون 
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("CRUD", $"{City.Name} Deleted from the System by {username} ", userId);
                // لهون هو تسجيل الlog  
                return Ok(new ApiResponse<string>
                {

                    Data = "done"
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbEmployee>>
                {
                    Message = ex.Message,

                });
            }
        }
    }
}
