using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Loujico.Models;

public partial class TbCompanyProduct
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public int ProductId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? StatusCp { get; set; }

    public decimal? TotalPrice { get; set; }
    [JsonIgnore]
    public virtual Co_Company_Name Company { get; set; } = null!;
    [JsonIgnore]
    public virtual TbProduct Product { get; set; } = null!;
}
