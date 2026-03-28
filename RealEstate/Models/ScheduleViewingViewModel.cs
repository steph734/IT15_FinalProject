using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models;

public class ScheduleViewingViewModel
{
    public int PropertyId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(120)]
    [Display(Name = "Full name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone")]
    public string? Phone { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Preferred date")]
    public DateTime PreferredDate { get; set; } = DateTime.Today.AddDays(1);

    [Display(Name = "Notes")]
    public string? Notes { get; set; }
}
