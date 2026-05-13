using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class BuyPropertyRequest
    {
        [Required]
        public int PropertyId { get; set; }

        public string PropertyType { get; set; } = "Buy"; // Buy or Rent

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string? Address { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public string? PaymentFrequency { get; set; } // Monthly or Yearly (for rent)
    }
}
