using Loujico.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using static Loujico.Models.TaskDTO;

namespace Loujico.BL
{
    public interface ITasks
    {
        public Task<(bool Success, string Message)> AddTaskAsync(CreateTaskDto dto);
        public  Task<(bool Success, string Message)> UpdateStatusAsync(string status, int taskId, int? employeeId, bool isPrivileged);
        public Task<(bool Success, string Message)> DeleteTaskAsync(int id, string currentUserId);
        public  Task<(List<TaskListDto> Data, int TotalCount)> GetTasksByProjectAsync(int projectId, int page, int pageSize, bool myTasks, int? employeeId, string? status, string? search);
        public Task<TaskDetailsDto?> GetTaskByIdAsync(int taskId);
        public  Task<(bool Success, string Message)> EditTaskAsync(int taskId, EditTaskDto dto, string username);
        public  Task<List<TbHistory>> LstEditHistory(int Pageid, int id, int count);

    }
    public class ClsTasks : ITasks
    {
        CompanySystemContext CTX;
        IHistory ClsHistory;
        Ilog ClsLogs;
        public ClsTasks(CompanySystemContext cTX,Ilog ilog,IHistory history)
        {
            ClsHistory = history;
            CTX = cTX;
            ClsLogs = ilog;

        }
        public async Task<(bool Success, string Message)> AddTaskAsync(CreateTaskDto dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.Title))
                return (false, "عنوان المهمة مطلوب.");

            if (dto.EstimatedHours < 1)
                return (false, "الوقت المتوقع يجب أن يكون على الأقل 1.");

            // Check project
            var projectExists = await CTX.TbProjects.AnyAsync(p => p.Id == dto.ProjectId);
            if (!projectExists)
                return (false, $"المشروع {dto.ProjectId} غير موجود.");

            // Check employees
            var assigned = dto.AssignedEmployees ?? new List<AssignEmployeeDto>();
            var employeeIds = assigned.Select(a => a.EmployeeId).Distinct().ToList();

            if (employeeIds.Any())
            {
                // جلب الموظفين المحددين مع UserId لكل واحد
                var employees = await CTX.TbEmployees
                    .Where(e => employeeIds.Contains(e.Id))
                    .Select(e => new { e.Id, e.UserId })
                    .ToListAsync();

                // الموظفين المفقودين (المعطاة أرقامهم غير موجودة في جدول الموظفين)
                var missing = employeeIds.Except(employees.Select(e => e.Id)).ToList();
                if (missing.Any())
                    return (false, $"الموظفون غير موجودين: {string.Join(", ", missing)}");

                // الموظفين الذين ليس لديهم حساب مستخدم مربوط (UserId = null أو فارغ)
                var noUserAssigned = employees
                    .Where(e => string.IsNullOrWhiteSpace(e.UserId))
                    .Select(e => e.Id)
                    .ToList();

                if (noUserAssigned.Any())
                {
                    // رسالة واضحة للمستخدم ليربط الموظفين بصفحة الموظفين
                    return (false, $"الموظفون التالية أرقامهم ليس لديهم حساب مستخدم مرتبط: {string.Join(", ", noUserAssigned)}. الرجاء ربطهم بحساب مستخدم من صفحة الموظفين قبل الإسناد.");
                }
            }

            await using var tx = await CTX.Database.BeginTransactionAsync();
            try
            {
                var task = new TbProjectTask
                {
                    ProjectId = dto.ProjectId,
                    Title = dto.Title,
                    Description = dto.Description,
                    EstimatedHours = dto.EstimatedHours,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                CTX.TbProjectTasks.Add(task);
                await CTX.SaveChangesAsync();

                // Assign employees
                var list = new List<TbProjectTaskEmployee>();
                foreach (var a in assigned)
                {
                    list.Add(new TbProjectTaskEmployee
                    {
                        TaskId = task.Id,
                        EmployeeId = a.EmployeeId,
                        RoleOnTask = a.RoleOnTask,
                        AssignedAt = DateTime.UtcNow
                    });
                }

                if (list.Any())
                {
                    CTX.TbProjectTaskEmployees.AddRange(list);
                    await CTX.SaveChangesAsync();
                }

                await tx.CommitAsync();
                return (true, "تمت إضافة المهمة بنجاح.");
            }
            catch
            {
                await tx.RollbackAsync();
                return (false, "حصل خطأ أثناء إنشاء المهمة.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteTaskAsync(int id,string currentUserId)
        {
            var task = await CTX.TbProjectTasks
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (task == null)
                return (false, $"المهمة {id} غير موجودة أو محذوفة مسبقًا.");

            task.IsDeleted = true;
            task.DeletedAt = DateTime.UtcNow;
            task.DeletedBy = currentUserId;
            task.UpdatedAt = DateTime.UtcNow;

            await CTX.SaveChangesAsync();

            return (true, "تم حذف المهمة (Soft Delete) بنجاح.");
        }

        public async Task<(bool Success, string Message)> UpdateStatusAsync(string status,int taskId,int? employeeId, bool isPrivileged )
        {
            try
            {
                // تحقق من قيمة الحالة على مستوى السيرفس أيضاً (دبل تشيك)
                var validStatuses = new[] { "Pending", "InProgress", "Completed", "Cancelled" };
                if (!validStatuses.Contains(status))
                    return (false, "the status must be Pending|InProgress|Completed|Cancelled");

                var task = await CTX.TbProjectTasks
                    .Include(t => t.TaskEmployees)
                    .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);

                if (task == null)
                    return (false, "المهمة غير موجودة");

                // إن لم يكن المستخدم Privileged، يجب أن يتوفر employeeId وأن يكون الموظف معيناً على المهمة
                if (!isPrivileged)
                {
                    if (!employeeId.HasValue)
                        return (false, "غير مسموح: EmployeeId مطلوب");

                    bool isAssigned = task.TaskEmployees.Any(te => te.EmployeeId == employeeId.Value && !te.IsDeleted);
                    if (!isAssigned)
                        return (false, "غير مسموح لك بتعديل هذه المهمة");
                }

                // تحديث الحالة والتوقيت
                task.Status = status;
                task.UpdatedAt = DateTime.UtcNow;

                await CTX.SaveChangesAsync();
                return (true, "تم تحديث الحالة بنجاح");
            }
            catch (Exception ex)
            {
                // سجّل الخطأ بطريقة مشروعك
                await ClsLogs.Add("Error", ex.Message, null);
                return (false, "حدث خطأ غير متوقع");
            }
        }

        public async Task<(List<TaskListDto> Data, int TotalCount)>GetTasksByProjectAsync(int projectId,int page,int pageSize,bool myTasks,int? employeeId,string? status,string? search)
        {
            var query = CTX.TbProjectTasks
                .Where(t => t.ProjectId == projectId && !t.IsDeleted);

            if (myTasks)
            {
                if (employeeId == null)
                    throw new Exception("EmployeeId is required when MyTasks = true");

                query = query.Where(t =>
                    t.TaskEmployees.Any(te => te.EmployeeId == employeeId));
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.Title.Contains(search));
            }

            int totalCount = await query.CountAsync();

            var tasks = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TaskListDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    EstimatedHours = t.EstimatedHours,

                    EmployeeNames = t.TaskEmployees
                        .Select(te => te.Employee.FirstName + " " + te.Employee.LastName)
                        .ToList()
                })
                .ToListAsync();

            return (tasks, totalCount);
        }


        public async Task<TaskDetailsDto?> GetTaskByIdAsync(int taskId)
        {
            var task = await CTX.TbProjectTasks
                .Where(t => t.Id == taskId && !t.IsDeleted)
                .Select(t => new TaskDetailsDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    EstimatedHours = t.EstimatedHours,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,

                    AssignedEmployees = t.TaskEmployees.Select(e =>
                        new TaskEmployeeResultDto
                        {
                            Id = e.Id,
                            EmployeeId = e.EmployeeId,
                            RoleOnTask = e.RoleOnTask,
                            AssignedAt = e.AssignedAt,
                            Name = e.Employee.FirstName + " " + e.Employee.LastName
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (task == null)
                return null;

            // جلب الملفات
            task.Files = await CTX.TbFiles
                .Where(f => f.EntityId == task.Id &&
                           
                            f.EntityType == tableName.Tasks &&
                            !f.IsDeleted).Select(a=> new TaskFileDto
                            {
                                FileName= a.FileName, 
                                Id = a.Id,
                                FileType =a.FileType,
                                UploadedAt=a.UploadedAt,
                            })
                .ToListAsync();
          
            return task;
        }

        public async Task<(bool Success, string Message)> EditTaskAsync(
       int taskId,
       EditTaskDto dto,
       string updatedBy)
        {
            var task = await CTX.TbProjectTasks
                .Include(t => t.TaskEmployees)
                .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);

            if (task == null)
                return (false, "المهمة غير موجودة");

            // 🔹 تحديث بيانات المهمة
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.EstimatedHours = dto.EstimatedHours;
            task.Status = dto.Status;
            task.UpdatedAt = DateTime.UtcNow;

            // 🔹 تعديل الموظفين فقط إذا وصلت قائمة غير فاضية
            if (dto.AssignedEmployees != null && dto.AssignedEmployees.Count!=0)
            {
                var incomingEmployeeIds = dto.AssignedEmployees
                    .Select(e => e.EmployeeId)
                    .Distinct()
                    .ToList();

                // 🔹 جلب الموظفين الموجودين فعليًا وغير محذوفين
                var validEmployees = await CTX.TbEmployees
                    .Where(e => incomingEmployeeIds.Contains(e.Id) && !e.IsDeleted)
                    .Select(e => e.Id)
                    .ToListAsync();

                if (!validEmployees.Any())
                    return (false, "لا يوجد موظفون صالحون للإضافة");

                // 1️⃣ حذف الموظفين اللي انشالوا
                var toRemove = task.TaskEmployees
                    .Where(te => !validEmployees.Contains(te.EmployeeId))
                    .ToList();

                if (toRemove.Any())
                    CTX.TbProjectTaskEmployees.RemoveRange(toRemove);

                // 2️⃣ إضافة / تعديل
                foreach (var emp in dto.AssignedEmployees
                             .Where(e => validEmployees.Contains(e.EmployeeId)))
                {
                    var existing = task.TaskEmployees
                        .FirstOrDefault(te => te.EmployeeId == emp.EmployeeId);

                    if (existing != null)
                    {
                        // 🔹 تعديل الدور
                        existing.RoleOnTask = emp.RoleOnTask;
                    }
                    else
                    {
                        // 🔹 إضافة موظف جديد
                        task.TaskEmployees.Add(new TbProjectTaskEmployee
                        {
                            EmployeeId = emp.EmployeeId,
                            RoleOnTask = emp.RoleOnTask,
                            AssignedAt = DateTime.UtcNow
                        });
                    }
                }
            await CTX.SaveChangesAsync();
            return (true, "تم تعديل المهمة بنجاح");
            }
            // ⛔ إذا القائمة فاضية أو null → تجاهل تعديل الموظفين نهائيًا
            else
            {
                return (false, "لم يتم ارسال الموظفين المطلوبين");
            }
        }


        public async Task<List<TbHistory>> LstEditHistory(int Pageid, int id, int count)
        {
            try
            {
                var LstProject = await ClsHistory.GetAllHistory(Pageid, id, tableName.Tasks, count);
                if (LstProject == null)
                {
                    return null;
                }
                else
                {
                    return LstProject;
                }
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return new List<TbHistory>();
            }
        }

    }
}
