using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Security.Claims;
using static Loujico.Models.TaskDTO;

namespace Loujico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        IFiles ClsFiles;
        ITasks ClsTasks;
        Ilog ClsLogs;
        IHistory ClsHistory;
        ICompanys ClsCompanys;
        CompanySystemContext CTX;
        UserManager<ApplicationUser> UserManager;
        public TaskController(ITasks ClsTask, CompanySystemContext context, UserManager<ApplicationUser> userManager, Ilog ilog, IHistory clsHistory, IFiles clsFiles, ICompanys clsCompanys)
        {
            ClsLogs = ilog;
            ClsTasks = ClsTask;
            CTX = context;
            UserManager = userManager;
            ClsHistory = clsHistory;
            ClsFiles = clsFiles;
            UserManager = userManager;
            ClsCompanys = clsCompanys;
        }

        [HttpGet("GetAll")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Team_Leader,Programmer")]

        public async Task<IActionResult> GetTasks([FromQuery]int projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool myTasks = false, [FromQuery] string? status=null,[FromQuery] string? search = null)
        {
            int? employeeId = null;

            if (myTasks)
            {
                var employeeIdClaim = User.FindFirst("EmployeeId");

                if (employeeIdClaim == null)
                    return Unauthorized("EmployeeId not found in token");

                employeeId = int.Parse(employeeIdClaim.Value);
            }

            var result = await ClsTasks.GetTasksByProjectAsync( projectId,page, pageSize, myTasks,employeeId,status,search);

            return Ok(new
            {
                Items = result.Item1,
                TotalCount = result.Item2
            });
        }

        [HttpPost("Add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Team_Leader")]
        public async Task<IActionResult> AddTask(CreateTaskDto dto)
        {

            var result = await ClsTasks.AddTaskAsync(dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });
            var usename = UserManager.GetUserName(User);
            var userId = UserManager.GetUserId(User);
            await ClsLogs.Add("CRUD", $"Task {dto.Title}  Added to the System by {usename} ", userId);

            return Ok("Done");
        }

        [HttpPatch("Edit")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Team_Leader")]
        public async Task<IActionResult> EditTaskWithFiles(int id,[FromForm] EditTaskDto dto,[FromForm] List<FileModel>? Data)
        {
            var username = UserManager.GetUserName(User);

            var result = await ClsTasks.EditTaskAsync(id, dto, username);
            if (!result.Success)
                return BadRequest(result.Message);

            if (Data != null)
            {
                foreach (var file in Data)
                    await ClsFiles.Add(file, "Tasks", id, tableName.Tasks,username);
            }

            return Ok("تم تعديل المهمة والملفات");
        }


        [HttpPatch("EditTaskStatus/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Team_Leader,Programmer")]
        public async Task<IActionResult> EditTaskStatus(
           int id,
           [FromQuery] string Status,
           [FromForm] List<FileModel>? Data)
        {
            // 1. اجلب اسم المستخدم و userId (للسجلات)
            var username = UserManager.GetUserName(User);
            var userId = UserManager.GetUserId(User);

            // 2. تحقق من صحة الحالة (يُعاد التحقق في السيرفس أيضاً)
            var validStatuses = new[] { "Pending", "InProgress", "Completed", "Cancelled" };
            if (!validStatuses.Contains(Status))
                return BadRequest("the status must be Pending|InProgress|Completed|Cancelled");

            // 3. استخرج EmployeeId من التوكن إن وُجد
            var employeeClaim = User.FindFirst("EmployeeId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            // ملاحظة: عادة EmployeeId هو Claim منفصل؛ ضمناً يمكن استعمال ClaimTypes.NameIdentifier لو ربطت المستخدم بموظف بطريقة مختلفة.
            int? employeeId = null;
            if (employeeClaim != null)
            {
                // حاول parse فقط إذا Claim هو رقم (EmployeeId عادة int)
                if (int.TryParse(employeeClaim.Value, out var parsedEmpId))
                    employeeId = parsedEmpId;
            }

            // 4. تحقق من الدور (هل هو Admin أو Team_Leader) --> هذا سيمنح صلاحية عامة
            bool isPrivileged = User.IsInRole("Admin") || User.IsInRole("Team_Leader");

            // 5. مناداة الـ service لتحديث الحالة
            var result = await ClsTasks.UpdateStatusAsync(Status, id, employeeId, isPrivileged);

            if (!result.Success)
                return BadRequest(result.Message); // أو BadRequest حسب الرسالة؛ هنا نستخدم Forbid لرفض الصلاحية

            // 6. لو التحديث نجح، أضف الملفات (إن وُجدت)
            if (Data != null && Data.Any())
            {
                foreach (var file in Data)
                {
                    var added = await ClsFiles.Add(file, "Tasks", id, tableName.Tasks, username);
                    if (!added)
                    {
                        // ملاحظة: الحالة تغيرت بالفعل. يمكنك هنا:
                        // - إرجاع خطأ وترك الحالة كما هي، أو
                        // - محاولة rollback إذا تدعم الـ files و DB معاً transaction (أصعب)
                        return BadRequest("The file could not be added");
                    }
                }
            }

            // 7. سجل الحدث
            await ClsLogs.Add("CRUD", $"Task :{id} has the status updated to {Status} by {username}", userId);

            return Ok("Done");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Team_Leader,Programmer")]
        [HttpDelete("DeleteFile/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteFile(int id)
        {
            try
            {
                var file = await ClsFiles.GetById(id, tableName.Tasks);
                if (file == null)
                    return NotFound("the file is deleted");

                // 1️⃣ صلاحيات عليا
                bool isPrivileged =
                    User.IsInRole("Admin") ||
                    User.IsInRole("Team_Leader");

                // 2️⃣ إن لم يكن Admin أو Team_Leader → تحقق من ملكية التاسك
                if (!isPrivileged)
                {
                    var employeeClaim = User.FindFirst("EmployeeId");
                    if (employeeClaim == null)
                        return Forbid("EmployeeId is required");

                    int employeeId = int.Parse(employeeClaim.Value);

                    bool isAssignedToTask = await CTX.TbProjectTaskEmployees
                        .AnyAsync(te =>
                            te.EmployeeId == employeeId &&
                            te.TaskId == file.EntityId
                        );

                    if (!isAssignedToTask)
                        return Forbid("You are not allowed to delete this file");
                }

                // 3️⃣ حذف الملف
                await ClsFiles.Delete(id, tableName.Tasks);

                // 4️⃣ Logging
                var username = UserManager.GetUserName(User);
                var userId = UserManager.GetUserId(User);

                await ClsLogs.Add(
                    "CRUD",
                    $"file {file.FileType} for Task {file.EntityId} deleted by {username}",
                    userId
                );

                return Ok(new ApiResponse<string>
                {
                    Data = "done"
                });
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return BadRequest(new ApiResponse<string>
                {
                    Message = ex.Message
                });
            }
        


    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Team_Leader,Programmer")]
        [HttpPost("AddFile")]
        public async Task<ActionResult<ApiResponse<string>>> AddFile([FromQuery]int id,[FromForm] List<FileModel>? Data)
        {
            var username = UserManager.GetUserName(User);
            var userId = UserManager.GetUserId(User);
            if (Data != null && Data.Any())
            {
                foreach (var file in Data)
                {
                    var added = await ClsFiles.Add(file, "Tasks", id, tableName.Tasks, username);
                    if (!added)
                    {
                        // ملاحظة: الحالة تغيرت بالفعل. يمكنك هنا:
                        // - إرجاع خطأ وترك الحالة كما هي، أو
                        // - محاولة rollback إذا تدعم الـ files و DB معاً transaction (أصعب)
                        return BadRequest("The file could not be added");
                    }
                    if (added)
                    {
                        await ClsLogs.Add("CRUD", $"File been added to the Task {id} by {username}", userId);
                        return Ok("File uplouded successfully");
                    }
                }

            }
            else
            {
                return BadRequest("please upload a file");
            }
                return BadRequest("an error in the system we will fix it");
        }

            [HttpDelete("Delete/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Team_Leader")]
        public async Task<IActionResult> DeleteTask(int id)
        {

            var userId = UserManager.GetUserId(User);
            var result = await ClsTasks.DeleteTaskAsync(id, userId);

            if (!result.Success)
                return NotFound(new { message = result.Message });
            var usename = UserManager.GetUserName(User);
            await ClsLogs.Add("CRUD", $"Task Id Deleted From the System by {usename} ", userId);
            return Ok(new { message = result.Message });
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await ClsTasks.GetTaskByIdAsync(id);

            if (task == null)
                return NotFound("المهمة غير موجودة");

            return Ok(task);
        }


        [HttpGet("EditHistory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<ActionResult<ApiResponse<List<TbHistory>>>> LstEditHistory([FromQuery] int page, [FromQuery] int id, [FromQuery] int count)
        {
            try
            {
                var history = await ClsTasks.LstEditHistory(page, id, count);
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


