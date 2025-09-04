using Microsoft.AspNetCore.Identity;

namespace Loujico.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfileImg { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }=false;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime LastVisit { get; set; }

    }
}
