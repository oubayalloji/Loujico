using System;
using System.Collections.Generic;

namespace Loujico.Models;

public partial class TbCustomersProduct
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? StatusCp { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual TbCustomer Customer { get; set; } = null!;

    public virtual TbProduct Product { get; set; } = null!;
}
