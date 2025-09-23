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

        [Required(ErrorMessage = "اسم الموظف مطلوب")]
        [StringLength(150)]
        public string FullName { get; set; }   // الاسم الكامل للموظف

        [Required(ErrorMessage = "المنصب مطلوب")]
        [StringLength(100)]
        public string Position { get; set; }   // المنصب (مدير قسم، سكرتير...)

        [StringLength(100)]
        public string Department { get; set; }   // القسم (مالية، تسويق...)

        [StringLength(20)]
        [RegularExpression(@"^[0-9+\-\s]{6,20}$", ErrorMessage = "رقم الهاتف غير صالح")]
        public string? Phone { get; set; }   // رقم الهاتف

        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [StringLength(150)]
        public string? Email { get; set; }   // البريد الإلكتروني

        [StringLength(500)]
        public string? Notes { get; set; }   // ملاحظات إضافية

    }
}
