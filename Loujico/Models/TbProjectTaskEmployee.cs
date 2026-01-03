using System.Text.Json.Serialization;

namespace Loujico.Models
{
    public partial class TbProjectTaskEmployee
    {
        public int Id { get; set; }

        public int TaskId { get; set; }
        public int EmployeeId { get; set; }

        public string? RoleOnTask { get; set; }  // دوره في المهمة (اختياري)

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public virtual TbProjectTask Task { get; set; } = null!;

        [JsonIgnore]
        public virtual TbEmployee Employee { get; set; } = null!;
    }

}
