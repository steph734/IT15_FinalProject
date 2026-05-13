using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models;

public class PropertyDocument
{
    [Key]
    public int DocumentId { get; set; }
    public int PropertyId { get; set; }
    public string DocumentUrl { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Property? Property { get; set; }
}
