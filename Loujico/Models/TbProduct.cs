using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public partial class TbProduct
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج ")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "اسم المنتج يجب أن يكون بين 2 و 150 حرف")]
        [RegularExpression(@"^[\p{L}\p{N}\s\-_]+$", ErrorMessage = "اسم المنتج يجب أن يحتوي على أحرف أو أرقام أو شرطات ومسافات فقط")]

        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "وصف المنتج مطلوب")]
        [StringLength(2000, ErrorMessage = "الوصف يجب ألا يتجاوز 2000 خانة")]

        public string ProductDescription { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]

        public decimal? Price { get; set; }

        [StringLength(50, ErrorMessage = "دورة الفوترة يجب ألا تتجاوز 50 خانة")]
        [RegularExpression(@"^(Monthly|Quarterly|Yearly|OneTime)?$", ErrorMessage = "دورة الفوترة يجب أن تكون Monthly أو Quarterly أو Yearly أو OneTime")]

        public string? BillingCycle { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public String? CreatedBy { get; set; }

        public String? UpdatedBy { get; set; }

        public virtual ICollection<TbCustomersProduct> TbCustomersProducts { get; set; } = new List<TbCustomersProduct>();

        public virtual ICollection<TbProductsEmployee> TbProductsEmployees { get; set; } = new List<TbProductsEmployee>();
    }
}