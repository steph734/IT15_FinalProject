using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models
{
    public class MarketingCampaign
    {
        [Key]
        public int CampaignId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string CampaignType { get; set; } = "Email"; // Email, SMS, Social Media, Direct Mail

        public string Status { get; set; } = "Draft"; // Draft, Scheduled, Active, Paused, Completed, Cancelled

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal Budget { get; set; }

        public decimal? ActualCost { get; set; }

        public int TargetAudienceCount { get; set; }

        public int SentCount { get; set; }

        public int OpenedCount { get; set; }

        public int ClickedCount { get; set; }

        public int ConvertedCount { get; set; }

        public double OpenRate { get; set; }

        public double ClickRate { get; set; }

        public double ConversionRate { get; set; }

        public string TargetSegment { get; set; } = string.Empty; // Customer segment name

        public int? CreatedBy { get; set; } // Employee ID
        [ForeignKey("CreatedBy")]
        public Employee? Creator { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }

        public string? TemplateId { get; set; }

        // Navigation properties
        public ICollection<CampaignRecipient> Recipients { get; set; } = new List<CampaignRecipient>();
        public ICollection<CampaignActivity> Activities { get; set; } = new List<CampaignActivity>();
    }

    public class CampaignRecipient
    {
        [Key]
        public int RecipientId { get; set; }

        public int CampaignId { get; set; }
        [ForeignKey("CampaignId")]
        public MarketingCampaign Campaign { get; set; } = null!;

        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending"; // Pending, Sent, Delivered, Opened, Clicked, Converted, Bounced, Failed

        public DateTime? SentAt { get; set; }

        public DateTime? OpenedAt { get; set; }

        public DateTime? ClickedAt { get; set; }

        public DateTime? ConvertedAt { get; set; }

        public string? Error { get; set; }
    }

    public class CampaignActivity
    {
        [Key]
        public int ActivityId { get; set; }

        public int CampaignId { get; set; }
        [ForeignKey("CampaignId")]
        public MarketingCampaign Campaign { get; set; } = null!;

        public string ActivityType { get; set; } = string.Empty; // Sent, Opened, Clicked, Converted, Bounced, Unsubscribed

        public string Description { get; set; } = string.Empty;

        public int? RecipientId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? Metadata { get; set; } // JSON for additional data
    }

    public class CustomerSegment
    {
        [Key]
        public int SegmentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Criteria { get; set; } = string.Empty; // JSON criteria for segmentation

        public int CustomerCount { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int? CreatedBy { get; set; } // Employee ID
        [ForeignKey("CreatedBy")]
        public Employee? Creator { get; set; }

        // Navigation properties
        public ICollection<SegmentMembership> Memberships { get; set; } = new List<SegmentMembership>();
    }

    public class SegmentMembership
    {
        [Key]
        public int MembershipId { get; set; }

        public int SegmentId { get; set; }
        [ForeignKey("SegmentId")]
        public CustomerSegment Segment { get; set; } = null!;

        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public string? AddedBy { get; set; } // Employee ID or System
    }
}
