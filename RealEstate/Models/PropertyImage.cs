using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models;

public class PropertyImage
{
    [Key]
    public int ImageId { get; set; }
    public int PropertyId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Property? Property { get; set; }
}
