using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        
        public decimal Amount { get; set; }
        
        public string PaymentMethod { get; set; }
        
        public string ReferenceNumber { get; set; }
        
        public string Status { get; set; }

        // Foreign key to Customer
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        // Foreign key to Property
        public int? PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        public Property Property { get; set; }

        // Property type (Buy or Rent)
        public string PropertyType { get; set; } = "Buy";

        // Payment frequency (Monthly or Yearly for rent)
        public string? PaymentFrequency { get; set; }

        // Customer details for reference
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ProcessedAt { get; set; }
    }
}
