using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models
{
    public class SupportTicket
    {
        [Key]
        public int TicketId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

        public string Status { get; set; } = "Open"; // Open, In Progress, Resolved, Closed, On Hold

        public string Category { get; set; } = "General"; // General, Technical, Billing, Property, Account

        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        public int? AssignedTo { get; set; } // Employee ID
        [ForeignKey("AssignedTo")]
        public Employee? AssignedEmployee { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; } // SLA due date

        public DateTime? ResolvedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public string Resolution { get; set; } = string.Empty;

        public int? RelatedPropertyId { get; set; }
        [ForeignKey("RelatedPropertyId")]
        public Property? RelatedProperty { get; set; }

        public string AttachmentUrls { get; set; } = string.Empty; // JSON array of URLs

        public bool IsEscalated { get; set; } = false;

        public DateTime? EscalatedAt { get; set; }

        public string EscalationReason { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
        public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
    }

    public class TicketComment
    {
        [Key]
        public int CommentId { get; set; }

        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public SupportTicket Ticket { get; set; } = null!;

        public string Comment { get; set; } = string.Empty;

        public int? AuthorId { get; set; } // Employee ID or Customer ID
        public string AuthorType { get; set; } = "Employee"; // Employee, Customer

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsInternal { get; set; } = false; // Internal notes not visible to customer
    }

    public class TicketAttachment
    {
        [Key]
        public int AttachmentId { get; set; }

        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public SupportTicket Ticket { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int? UploadedBy { get; set; } // Employee ID
    }
}
