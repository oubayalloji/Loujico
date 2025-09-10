using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Loujico.Models;

public partial class TbProductsEmployee
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int EmployeeId { get; set; }

    public string? RoleOnProduct { get; set; }

    public DateTime JoinedAt { get; set; }

    [JsonIgnore]
    public virtual TbEmployee Employee { get; set; } = null!;

    [JsonIgnore] 
    public virtual TbProduct Product { get; set; } = null!;
}
