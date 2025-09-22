using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class Co_Activity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم النشاط مطلوب")]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int IndustryId { get; set; }

        [ForeignKey("IndustryId")]
        public Co_Industry Industry { get; set; }

        [ForeignKey("CompanyId")]
        public Co_Company_Name Company { get; set; }
    }
}
