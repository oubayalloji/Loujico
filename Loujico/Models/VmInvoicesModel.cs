using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{

    public class VmInvoicesModel : IValidatableObject
    {

        [Range(1, int.MaxValue, ErrorMessage = "معرّف الفاتورة يجب أن يكون قيمة صحيحة موجبة")]
        public int Id { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "معرّف العميل يجب أن يكون قيمة صحيحة موجبة")]
        public int? CustomerId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "معرّف المشروع يجب أن يكون قيمة صحيحة موجبة في حال التحديد")]
        public int? ProjectId { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Amount { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly? InvoicesDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly? DueDate { get; set; }
        [StringLength(20, ErrorMessage = "حالة الفاتورة يجب أن لا تتجاوز 20 حرفاً")]
        [RegularExpression(@"^(Pending|Paid|Overdue|Cancelled)$",
            ErrorMessage = "حالة الفاتورة يجب أن تكون: Pending أو Paid أو Overdue أو Cancelled")]

        public string? InvoiceStatus { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // التحقق من أن تاريخ الاستحقاق بعد أو يساوي تاريخ الفاتورة
            if (InvoicesDate.HasValue && DueDate.HasValue && DueDate.Value < InvoicesDate.Value)
            {
                yield return new ValidationResult(
                    "تاريخ الاستحقاق يجب أن يكون بعد أو يساوي تاريخ الفاتورة",
                    new[] { nameof(DueDate), nameof(InvoicesDate) });
            }

            // التحقق من أن المبلغ أكبر من صفر
            if (Amount.HasValue && Amount.Value <= 0)
            {
                yield return new ValidationResult(
                    "المبلغ يجب أن يكون أكبر من صفر",
                    new[] { nameof(Amount) });
            }

        }
    }
}
