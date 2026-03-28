using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models;

public class TransactionFormModel
{
    public int PropertyId { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(120)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Mobile number")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(250)]
    [Display(Name = "Street address")]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(80)]
    [Display(Name = "City / Municipality")]
    public string? City { get; set; }

    [StringLength(80)]
    [Display(Name = "Province")]
    public string? Province { get; set; }

    [StringLength(20)]
    [Display(Name = "ZIP code")]
    public string? PostalCode { get; set; }

    [Required(ErrorMessage = "Select a payment method.")]
    [Display(Name = "Payment method")]
    public string PaymentMethod { get; set; } = string.Empty;

    [StringLength(120)]
    [Display(Name = "Account holder name (as on bank / e-wallet)")]
    public string? PaymentAccountName { get; set; }

    [StringLength(80)]
    [Display(Name = "Reference / transaction number (after payment)")]
    public string? PaymentReferenceNumber { get; set; }

    [Display(Name = "I agree to the reservation terms and understand this is a demo checkout.")]
    public bool AcceptTerms { get; set; }
}
