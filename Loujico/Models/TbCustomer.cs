using System;
using System.Collections.Generic;

namespace Loujico.Models;

public partial class TbCustomer
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string CustomerAddress { get; set; } = null!;

    public string CompanyDescription { get; set; } = null!;

    public string? Industry { get; set; }

    public string? ServiceProvided { get; set; }

    public string? Inquiry { get; set; }

    public DateOnly? WorkDate { get; set; }

    public int? WorkDuration { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastVisit { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<TbCustomersProduct> TbCustomersProducts { get; set; } = new List<TbCustomersProduct>();

    public virtual ICollection<TbInvoice> TbInvoices { get; set; } = new List<TbInvoice>();

    public virtual ICollection<TbProject> TbProjects { get; set; } = new List<TbProject>();
}
