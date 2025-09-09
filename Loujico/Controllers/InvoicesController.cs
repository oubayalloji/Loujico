using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Loujico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

    public class InvoicesController : ControllerBase
    {
        CompanySystemContext CTX;
        IInvoices ClsInvoices;
        Ilog ClsLogs;
        IHistory ClsHistory;
        IFiles ClsFiles;
        UserManager<ApplicationUser> UserManager;

        public InvoicesController(CompanySystemContext cTX, IInvoices clsInvoices, Ilog clsLogs, IHistory clsHistory, UserManager<ApplicationUser> userManager, IFiles clsFiles)
        {
            CTX = cTX;
            ClsInvoices = clsInvoices;
            ClsLogs = clsLogs;
            ClsHistory = clsHistory;
            UserManager = userManager;
            ClsFiles = clsFiles;
        }

        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromForm] TbInvoice invoice, [FromForm] List<FileModel>? Data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> { Message = "wronge" });
            }

            try
            {
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                invoice.CreatedBy = username;
                await ClsInvoices.Add(invoice);
                await ClsLogs.Add("Error", $"{invoice.Id} added to the System by {username}", userId);
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Invoices", invoice.Id, tableName.invoice);
                    }
                }
                return Ok(new ApiResponse<string> { Message = "Done" });
            }

            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }

        [HttpPatch("Edit")]
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromForm] TbInvoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> { Message = "wronge" });
            }

            try
            {
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                invoice.UpdatedBy = username;
                await ClsInvoices.Edit(invoice);
                await ClsLogs.Add("Error", $"id : {invoice.Id} with name : {invoice.Id} updated to the System by {username}", userId);

                return Ok(new ApiResponse<string> { Message = "Done" });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                var invoice = await ClsInvoices.GetById(id);
                await ClsInvoices.Delete(id);
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                await ClsLogs.Add("Error", $"{invoice.Invoice.Id} Deleted from the System by {username}", userId);

                return Ok(new ApiResponse<string> { Data = "done" });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string> { Message = ex.Message });
            }
        }

        [HttpGet("GetAll/{id}")]
        public async Task<ActionResult<ApiResponse<List<TbInvoice>>>> GetAll(int id)
        {
            try
            {
                return Ok(new ApiResponse<List<TbInvoice>> { Data = await ClsInvoices.GetAll(id) });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbInvoice>> { Message = ex.Message });
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ApiResponse<InvoiceModel>>> GetById(int id)
        {
            try
            {
                var invoice = await ClsInvoices.GetById(id);
                return Ok(new ApiResponse<InvoiceModel> { Data = invoice });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<InvoiceModel> { Message = ex.Message });
            }
        }
        [HttpGet("EditHistory/{page}/{id}")]
        public async Task<ActionResult<ApiResponse<List<TbHistory>>>> LstEditHistory(int page, int id)
        {
            try
            {
                var history = await ClsInvoices.LstEditHistory(page, id);
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