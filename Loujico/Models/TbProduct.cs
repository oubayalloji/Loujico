 using System;
using System.Collections.Generic;

namespace Loujico.Models;

public partial class TbProduct
{
    public int Id { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductDescription { get; set; } = null!;

    public decimal? Price { get; set; }

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
