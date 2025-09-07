// من هون 
using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var username = UserManager.GetUserName(User);
var userId = UserManager.GetUserId(User);
await ClsLogs.Add("Error", $"{Customer.CustomerName} Deleted from the System by {username} ", userId);
// لهون هو تسجيل الlog  
return Ok(new ApiResponse<String>
{
    Data = "done"
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
        [HttpGet("GetById/{id}")]
public async Task<ActionResult<ApiResponse<string>>> GetById(int id)
{
    try
    {
        var Customerloyee = await ClsCustomers.GetById(id);

        return Ok(new ApiResponse<TbCustomer>
        {
            Data = Customerloyee
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
    }
}