using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace Loujico.Models
{
    public partial class TbCustomer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يرجى أدخال اسم العميل ")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "اسم العميل يجب أن يكون بين 2 و 100 حرف")]
        [RegularExpression(@"^[\p{L}\p{N}\s\-_]+$", ErrorMessage = "اسم العميل يحتوي على أحرف أو أرقام أو شرطات ومسافات فقط")]

        public string CustomerName { get; set; } = null!;


        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^[0-9+\-\s]{6,20}$", ErrorMessage = "رقم الهاتف غير صالح")] // أقوى من 
        [StringLength(20, ErrorMessage = "رقم الهاتف يجب ألا يتجاوز 20 خانة")]
        public string Phone { get; set; } = null!;




        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [StringLength(150, ErrorMessage = "البريد الإلكتروني يجب ألا يتجاوز 150 خانة")]

        public string? Email { get; set; }

        [Required(ErrorMessage = "عنوان العميل مطلوب")]
        [StringLength(255, ErrorMessage = "عنوان العميل يجب ألا يتجاوز 255 خانة")]

        public string CustomerAddress { get; set; } = null!;

        [Required(ErrorMessage = "وصف الشركة مطلوب")]
        [StringLength(1000, ErrorMessage = "الوصف يجب ألا يتجاوز 1000 خانة")]

        public string CompanyDescription { get; set; } = null!;

        [StringLength(100, ErrorMessage = "اسم الصناعة يجب ألا يتجاوز 100 خانة")]

        public string? Industry { get; set; }

        [StringLength(150, ErrorMessage = "الخدمة المقدمة يجب ألا تتجاوز 150 خانة")]

        public string? ServiceProvided { get; set; }

        [StringLength(1000, ErrorMessage = "الاستفسار يجب ألا يتجاوز 1000 خانة")]

        public string? Inquiry { get; set; }


        public DateOnly? WorkDate { get; set; }

        [Range(1, 3650, ErrorMessage = "مدة العمل يجب أن تكون بين 1 و 3650 يوم")]

        public int? WorkDuration { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastVisit { get; set; }

        public bool IsDeleted { get; set; }


        public String? CreatedBy { get; set; }

        public String? UpdatedBy { get; set; }
        public virtual ICollection<TbCustomersProduct> TbCustomersProducts { get; set; } = new List<TbCustomersProduct>();

        public virtual ICollection<TbInvoice> TbInvoices { get; set; } = new List<TbInvoice>();

        public virtual ICollection<TbProject> TbProjects { get; set; } = new List<TbProject>();
    }
}
