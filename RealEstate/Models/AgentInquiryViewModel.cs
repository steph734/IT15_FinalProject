using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models;

public class AgentInquiryViewModel
{
    public int PropertyId { get; set; }
    public int? AgentId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Message { get; set; } = string.Empty;
}