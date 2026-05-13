using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Required for ForeignKey

namespace RealEstate.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        public int TransactionId { get; set; }

        [ForeignKey("TransactionId")] // Explicitly links the ID to the Object
        public Transaction Transaction { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")] // Explicitly links the ID to the Object
        public Employee Employee { get; set; }

        public decimal TotalAmount { get; set; }
        public DateTime DateIssued { get; set; }
        public string Status { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}