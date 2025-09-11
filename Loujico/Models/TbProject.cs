using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Loujico.Models
{
    public partial class TbProject : IValidatableObject
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "عنوان المشروع مطلوب")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "عنوان المشروع يجب أن يكون بين 2 و 150 حرف")]
        [RegularExpression(@"^[\p{L}\p{N}\s\-_]+$", ErrorMessage = "العنوان يجب أن يحتوي على أحرف أو أرقام أو شرطات ومسافات فقط")]

        public string Title { get; set; } = null!;

        [StringLength(2000, ErrorMessage = "الوصف يجب ألا يتجاوز 2000 خانة")]

        public string? ProjectDescription { get; set; }

        [StringLength(100, ErrorMessage = "نوع المشروع يجب ألا يتجاوز 100 خانة")]

        public string? ProjectType { get; set; }

        public int CustomerId { get; set; }

        [Range(0, 100, ErrorMessage = "نسبة الإنجاز يجب أن تكون بين 0 و 100")]

        public int Progress { get; set; }

        [Required(ErrorMessage = "تاريخ بدء المشروع مطلوب")]

        public DateOnly StartDate { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]

        public decimal? Price { get; set; }

        [Required(ErrorMessage = "تاريخ الانتهاء مطلوب")]

        public DateOnly? EndDate { get; set; }

        [StringLength(50, ErrorMessage = "حالة المشروع يجب ألا تتجاوز 50 خانة")]
        [RegularExpression(@"^(Pending|Active|Completed|Cancelled)?$", ErrorMessage = "الحالة يجب أن تكون: Pending أو Active أو Completed أو Cancelled")]
   
        public string? ProjectStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastVisit { get; set; }

        public bool IsDeleted { get; set; }

        public String? CreatedBy { get; set; }

        public String? UpdatedBy { get; set; }
        [JsonIgnore]
        public virtual TbCustomer Customer { get; set; } = null!;

        public virtual ICollection<TbInvoice> TbInvoices { get; set; } = new List<TbInvoice>();

        public virtual ICollection<TbProjectsEmployee> TbProjectsEmployees { get; set; } = new List<TbProjectsEmployee>();
     
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate.HasValue && EndDate.Value < StartDate)
            {
                yield return new ValidationResult("تاريخ الانتهاء يجب أن يكون بعد أو يساوي تاريخ البدء", new[] { nameof(EndDate), nameof(StartDate) });
            }

            if (Price.HasValue && Price.Value <= 0)
            {
                yield return new ValidationResult("السعر يجب أن يكون أكبر من صفر", new[] { nameof(Price) });
            }
        }
    }
}