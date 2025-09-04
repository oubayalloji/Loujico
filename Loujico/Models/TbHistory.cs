using System;
using System.Collections.Generic;

namespace Loujico.Models;

public partial class TbHistory
{
    public int Id { get; set; }

    public string TableName { get; set; } = null!;

    public int RecordId { get; set; }

    public string ColumnName { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string ActionType { get; set; } = null!;

    public DateTime ActionTime { get; set; }
}
