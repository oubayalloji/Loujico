using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class Co_Contact
    {
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Co_Company_Name Company { get; set; }

        [Required]
        public int ContactTypeId { get; set; }

        [ForeignKey("ContactTypeId")]
        public TbContact ContactType { get; set; }

        [Required(ErrorMessage = "معلومة الاتصال مطلوبة")]
        [StringLength(150)]
        public string Name { get; set; }

        public int? EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Co_CompanyEmployee Employee { get; set; }
    }
}
