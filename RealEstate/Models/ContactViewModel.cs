using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models;

public class ContactViewModel
{
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(120)]
    [Display(Name = "Full name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(40)]
    [Display(Name = "Phone")]
    public string? Phone { get; set; }

    [StringLength(200)]
    [Display(Name = "Subject")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Please enter a message.")]
    [StringLength(4000)]
    [Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;

    public int? AgentId { get; set; }
    public int? PropertyId { get; set; }
}
