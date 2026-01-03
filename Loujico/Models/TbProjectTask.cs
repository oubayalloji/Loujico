using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Loujico.Models
{
    public partial class TbProjectTask
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }   // المشروع الذي تنتمي إليه المهمة

        [Required(ErrorMessage = "عنوان المهمة مطلوب")]
        [StringLength(150)]
        public string Title { get; set; } = null!;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Range(1, 10000, ErrorMessage = "الوقت المتوقع يجب أن يكون على الأقل 1 ساعة")]
        public int EstimatedHours { get; set; }   // الوقت المتوقع لإنجاز المهمة

        [RegularExpression(@"^(Pending|InProgress|Completed|Cancelled)$",
            ErrorMessage = "الحالة يجب أن تكون: Pending أو InProgress أو Completed أو Cancelled")]
        public string Status { get; set; } = "Pending";
        public string? Resources { get; set; } 
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        public virtual TbProject Project { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<TbProjectTaskEmployee> TaskEmployees { get; set; }
            = new List<TbProjectTaskEmployee>();
    }

}
