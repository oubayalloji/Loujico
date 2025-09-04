using System;
using System.Collections.Generic;

namespace Loujico.Models;

public partial class TbProductsEmployee
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int EmployeeId { get; set; }

    public string? RoleOnProduct { get; set; }

    public DateTime JoinedAt { get; set; }

    public virtual TbEmployee Employee { get; set; } = null!;

    public virtual TbProduct Product { get; set; } = null!;
}
