using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models
{
    public class SalesOpportunity
    {
        [Key]
        public int OpportunityId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Stage { get; set; } = "Lead"; // Lead, Qualified, Proposal, Negotiation, Closed Won, Closed Lost

        public decimal EstimatedValue { get; set; }

        public decimal? ActualValue { get; set; }

        public int Probability { get; set; } = 50; // 0-100%

        public DateTime ExpectedCloseDate { get; set; }

        public DateTime? ActualCloseDate { get; set; }

        public string Source { get; set; } = string.Empty; // Website, Referral, Cold Call, Marketing, etc.

        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        public int? PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        public Property? Property { get; set; }

        public int? AssignedTo { get; set; } // Employee ID (Agent/Broker)
        [ForeignKey("AssignedTo")]
        public Employee? AssignedEmployee { get; set; }

        public int? CreatedBy { get; set; } // Employee ID
        [ForeignKey("CreatedBy")]
        public Employee? Creator { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }

        public string? LostReason { get; set; }

        public string? NextStep { get; set; }

        public DateTime? FollowUpDate { get; set; }

        // Navigation properties
        public ICollection<OpportunityActivity> Activities { get; set; } = new List<OpportunityActivity>();
        public ICollection<OpportunityNote> Notes { get; set; } = new List<OpportunityNote>();
    }

    public class OpportunityActivity
    {
        [Key]
        public int ActivityId { get; set; }

        public int OpportunityId { get; set; }
        [ForeignKey("OpportunityId")]
        public SalesOpportunity Opportunity { get; set; } = null!;

        public string ActivityType { get; set; } = string.Empty; // Call, Email, Meeting, Note, Task

        public string Subject { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime ActivityDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled

        public int? PerformedBy { get; set; } // Employee ID
        [ForeignKey("PerformedBy")]
        public Employee? Performer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class OpportunityNote
    {
        [Key]
        public int NoteId { get; set; }

        public int OpportunityId { get; set; }
        [ForeignKey("OpportunityId")]
        public SalesOpportunity Opportunity { get; set; } = null!;

        public string Note { get; set; } = string.Empty;

        public int? CreatedBy { get; set; } // Employee ID
        [ForeignKey("CreatedBy")]
        public Employee? Creator { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsPrivate { get; set; } = false;
    }

    public class SalesPipeline
    {
        [Key]
        public int PipelineId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Stages { get; set; } = string.Empty; // JSON array of stage names

        public bool IsDefault { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; } // Employee ID
        [ForeignKey("CreatedBy")]
        public Employee? Creator { get; set; }

        // Navigation properties
        public ICollection<SalesOpportunity> Opportunities { get; set; } = new List<SalesOpportunity>();
    }
}
