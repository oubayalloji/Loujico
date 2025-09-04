using System;
using System.Collections.Generic;

namespace Loujico.Models;

public partial class TbFile
{
    public int Id { get; set; }

    public string EntityType { get; set; } = null!;

    public int EntityId { get; set; }

    public string FileName { get; set; } = null!;

    public string? FileType { get; set; }

    public DateTime UploadedAt { get; set; }

    public int? UploadedBy { get; set; }
}
