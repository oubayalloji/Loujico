using System;
using System.Collections.Generic;

namespace Loujico.Models;

public partial class TbProjectsEmployee
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int EmployeeId { get; set; }

    public string? RoleOnProject { get; set; }

    public DateTime JoinedAt { get; set; }

    public virtual TbEmployee Employee { get; set; } = null!;

    public virtual TbProject Project { get; set; } = null!;
}
