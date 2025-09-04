using System;
using System.Collections.Generic;

namespace Loujico.Models;

public partial class TbLog
{
    public int Id { get; set; }

    public string? ActionType { get; set; }

    public string? Action { get; set; }

    public DateTime TimeStamp { get; set; }

    public string? UserId { get; set; }
}
