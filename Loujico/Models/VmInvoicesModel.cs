using System.ComponentModel.DataAnnotations;

namespace Loujico.Models
{
    public class VmInvoicesModel
    {
        public int? CustomerId { get; set; }

        public int? ProjectId { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        public decimal? Amount { get; set; }
        [Required(ErrorMessage = "تاريخ الفاتورة مطلوب")]
        public DateOnly? InvoicesDate { get; set; }
        [Required(ErrorMessage = "تاريخ الاستحقاق مطلوب")]
        public DateOnly? DueDate { get; set; }
        [StringLength(50, ErrorMessage = "حالة الفاتورة يجب ألا تتجاوز 50 خانة")]
        [RegularExpression(@"^(Pending|Paid|Overdue|Cancelled)?$", ErrorMessage = "الحالة يجب أن تكون: Pending أو Paid أو Overdue أو Cancelled")]
        public string? InvoiceStatus { get; set; }
    }
}
