using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? ZipCode { get; set; }

        public string? Country { get; set; }

        public string PropertyType { get; set; } = "Residential";

        public string? InterestedProperties { get; set; }

        public decimal? MinBudget { get; set; }

        public decimal? MaxBudget { get; set; }

        public string Status { get; set; } = "Interested";

        public string PaymentMethod { get; set; } = "Mastercard";

        public string? CardholderName { get; set; }

        public string? CardNumber { get; set; }

        public string? ExpiryDate { get; set; }

        public string? CVV { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastContactedDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int? BrokerId { get; set; }

        // Foreign key to Property (for purchased property)
        public int? PurchasedPropertyId { get; set; }
        [ForeignKey("PurchasedPropertyId")]
        public Property? PurchasedProperty { get; set; }

        // Navigation property for payments
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
