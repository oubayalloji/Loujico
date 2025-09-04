using System;
using System.Collections.Generic;

namespace Loujico.Models;

public partial class TbEmployee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string? EmployeesAddress { get; set; }

    public string? Position { get; set; }

    public int Age { get; set; }

    public string? ProfileImage { get; set; }

    public bool IsPresent { get; set; }

    public string? EmployeesDescription { get; set; }

    public int? ServiceDuration { get; set; }

    public decimal? Salary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastVisit { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<TbProductsEmployee>? TbProductsEmployees { get; set; } = new List<TbProductsEmployee>();

    public virtual ICollection<TbProjectsEmployee>? TbProjectsEmployees { get; set; } = new List<TbProjectsEmployee>();
}
