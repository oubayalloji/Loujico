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
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] VmInvoicesModel invoice, [FromForm] List<FileModel>? Data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> { Message = "wronge" });
            }

            try
            {
               
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                TbInvoice tbInvoice = new TbInvoice{
                    Amount=invoice.Amount,
                    CreatedAt=DateTime.Now,
                    CreatedBy=username,
                    DueDate=invoice.DueDate,    
                    CustomerId = invoice.CustomerId,
                    InvoicesDate = invoice.InvoicesDate,

               
                };

                await ClsInvoices.Add(tbInvoice);
                await ClsLogs.Add("Error", $"{tbInvoice.Id} added to the System by {username}", userId);
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Invoices", tbInvoice.Id, tableName.invoice);
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
        public async Task<ActionResult<ApiResponse<string>>> Edit([FromBody] VmInvoicesModel invoice,[FromForm] List<FileModel>? Data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> { Message = "wronge" });
            }

            try
            {
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);
                TbInvoice tbInvoice = new TbInvoice
                {
                    Amount = invoice.Amount,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = username,
                    DueDate = invoice.DueDate,
                    CustomerId = invoice.CustomerId,
                    InvoicesDate = invoice.InvoicesDate,
                    UpdatedBy=username,

                };
                await ClsInvoices.Edit(tbInvoice);
                await ClsLogs.Add("Error", $"id : {tbInvoice.Id} with name : {tbInvoice.Id} updated to the System by {username}", userId);
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        await ClsFiles.Add(item, "Invoices", tbInvoice.Id, tableName.invoice);
                        await ClsLogs.Add("CRUD", $"file {item.fileType} added to : {tbInvoice.Id} by {username} ", userId);

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
        [HttpDelete("DeleteFile/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteFile(int id)
        {
            try
            {
                var file = await ClsFiles.GetById(id, tableName.invoice);
                await ClsFiles.Delete(id, tableName.invoice);

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
        [HttpDelete("Delete/{id}")]
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

        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<List<TbInvoice>>>> GetAll([FromQuery] int Page, [FromQuery] int Count)
        {
            try
            {
                return Ok(new ApiResponse<List<TbInvoice>> { Data = await ClsInvoices.GetAll(Page,Count) });
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
        [HttpGet("EditHistory")]
        public async Task<ActionResult<ApiResponse<List<TbHistory>>>> LstEditHistory([FromQuery] int page, [FromQuery] int id, [FromQuery] int count)
        {
            try 
            {
                var history = await ClsInvoices.LstEditHistory(page, id, count);
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
                var Invoice = await ClsInvoices.Search(name, page, count);

                return Ok(new ApiResponse<object>
                {
                    Data = Invoice
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<List<TbInvoice>>
                {
                    Message = ex.Message,

                });
            }

        }
    }
}