using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Loujico.Models;
public partial class TbInvoice : IValidatableObject
{
    public int Id { get; set; }

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

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastVisit { get; set; }

    public bool IsDeleted { get; set; }

    public String? CreatedBy { get; set; }

    public String? UpdatedBy { get; set; }

    public virtual TbCustomer? Customer { get; set; }

    public virtual TbProject? Project { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (InvoicesDate.HasValue && DueDate.HasValue && DueDate.Value < InvoicesDate.Value)
        {
            yield return new ValidationResult("تاريخ الاستحقاق يجب أن يكون بعد أو يساوي تاريخ الفاتورة", new[] { nameof(DueDate), nameof(InvoicesDate) });
        }

        if (Amount.HasValue && Amount.Value <= 0)
        {
            yield return new ValidationResult("المبلغ يجب أن يكون أكبر من صفر", new[] { nameof(Amount) });
        }
    }
}
