using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class TaskDTO
    {
        public class CreateTaskDto
        {
            public int ProjectId { get; set; }

            public string Title { get; set; } = null!;

            public string? Description { get; set; }

            public int EstimatedHours { get; set; }
            public List<AssignEmployeeDto>? AssignedEmployees { get; set; }
        }

        public class AssignEmployeeDto
        {
            public int EmployeeId { get; set; }
            public string? RoleOnTask { get; set; }
        }

        public class TaskResultDto
        {
            public int Id { get; set; }
            public int ProjectId { get; set; }
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
            public int EstimatedHours { get; set; }
            public string Status { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public List<TaskEmployeeResultDto> AssignedEmployees { get; set; } = new();
        }

        public class TaskEmployeeResultDto
        {
            public int Id { get; set; }
            public int EmployeeId { get; set; }
            public string? RoleOnTask { get; set; }
            public string? Name { get; set; }
            public DateTime AssignedAt { get; set; }
        }
        public class TaskListDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public string Status { get; set; } = null!;
            public int EstimatedHours { get; set; }

            public List<string> EmployeeNames { get; set; } = new();
        }

        public class TaskDetailsDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
            public int EstimatedHours { get; set; }

            [RegularExpression(@"^(Pending|InProgress|Completed|Cancelled)$",
                ErrorMessage = "الحالة يجب أن تكون: Pending أو InProgress أو Completed أو Cancelled")]
            public string Status { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public List<TaskEmployeeResultDto> AssignedEmployees { get; set; }
                = new();

            public List<TaskFileDto> Files { get; set; } = new();
        }
        public class TaskFileDto
        {
            public int Id { get; set; }
            public string FileName { get; set; }       // الاسم الأصلي
           // رابط التحميل
            public string FileType { get; set; }       // image / pdf / doc
     
            public DateTime UploadedAt { get; set; }
            
        }
        public class EditTaskDto
        {
            [Required]
            public string Title { get; set; }

            public string? Description { get; set; }
            [Required]
            public int EstimatedHours { get; set; }
            [Required]

            [RegularExpression(@"^(Pending|InProgress|Completed|Cancelled)$",
            ErrorMessage = "الحالة يجب أن تكون: Pending أو InProgress أو Completed أو Cancelled")]
            public string Status { get; set; }

            [Required]
            public List<AssignEmployeeDto>? AssignedEmployees { get; set; }
        }


    }
}
