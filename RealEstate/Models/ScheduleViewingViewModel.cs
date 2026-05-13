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

    [Display(Name = "Profile Photo")]
    [StringLength(500)]
    public string? CustomerPhotoUrl { get; set; }

    public int? CustomerId { get; set; } // Link to Customer

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Preferred date")]
    public DateTime PreferredDate { get; set; } = DateTime.Today.AddDays(1);

    [Display(Name = "Preferred time")]
    public string? PreferredTime { get; set; }

    [Display(Name = "Number of visitors")]
    public int NumberOfVisitors { get; set; } = 1;

    [Display(Name = "Buyer type")]
    public string? BuyerType { get; set; }

    [Display(Name = "Financing status")]
    public string? FinancingStatus { get; set; }

    [Display(Name = "How did you hear about us?")]
    public string? InformationSource { get; set; }

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Appointment Type")]
    [StringLength(20)]
    public string? AppointmentType { get; set; } = "self"; // "self" or "pickup"

    [Display(Name = "Your Address")]
    [StringLength(200)]
    public string? CustomerAddress { get; set; }
}

