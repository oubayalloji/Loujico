using System;
using System.Collections.Generic;

namespace Loujico.Models;

public partial class TbProject
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? ProjectDescription { get; set; }
    public string? ProjectType { get; set; }

    public int CustomerId { get; set; }
    public int Progress { get; set; }

    public DateOnly StartDate { get; set; }

    public decimal? Price { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? ProjectStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastVisit { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual TbCustomer Customer { get; set; } = null!;

    public virtual ICollection<TbInvoice> TbInvoices { get; set; } = new List<TbInvoice>();

    public virtual ICollection<TbProjectsEmployee> TbProjectsEmployees { get; set; } = new List<TbProjectsEmployee>();
}
