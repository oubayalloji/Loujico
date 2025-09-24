using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class Co_CompanyEmployee
    {
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }   // المفتاح الأجنبي لربط الموظف بالشركة

        [ForeignKey("CompanyId")]
        public Co_Company_Name Company { get; set; }

        [Required(ErrorMessage = "اسم الموظف الأول مطلوب")]
        [StringLength(150)]
        public string FirstName { get; set; }   // الاسم الكامل للموظف

        [Required(ErrorMessage = "اسم الموظف الثاني مطلوب")]
        [StringLength(150)]
        public string LastName { get; set; }   // الاسم الكامل للموظف

        [Required(ErrorMessage = "المنصب مطلوب")]
        [StringLength(100)]
        public string Position { get; set; }   // المنصب (مدير قسم، سكرتير...)

        [StringLength(100)]
        public string Department { get; set; }   // القسم (مالية، تسويق...)

        [StringLength(500)]
        public string? Notes { get; set; }   // ملاحظات إضافية

        public ICollection<Co_Contact> Contacts { get; set; }

    }
}
