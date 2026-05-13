using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    /// <summary>
    /// Main Property model - central entity for the real estate system
    /// </summary>
    public class Property
    {
        public int PropertyId { get; set; }
        public int Id => PropertyId;

        // Seller / Agent
        public int? SellerId { get; set; }
        public PropertySeller? Seller { get; set; }

        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int? ApprovedBy { get; set; }
        public ApplicationUser? Approver { get; set; }

        // Agent Assignment (one agent can have many properties)
        public int? AgentId { get; set; }
        public Agent? Agent { get; set; }

        // Property Details
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Accommodation { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Available, Sold, Rejected

        // Type & Listing
        public string PropertyType { get; set; } = string.Empty;
        public PropertyListingType ListingType { get; set; }

        // Pricing
        public decimal BasePrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal Price => BasePrice;

        // RentCast API Data - Investment Analysis
        public decimal? YieldScore { get; set; } // Cap Rate percentage
        public decimal? MarketValue { get; set; } // From RentCast API
        public decimal? RentEstimate { get; set; } // Monthly rent estimate
        public string? ProfitabilityRating { get; set; } // Excellent, Good, Fair, Poor
        public DateTime? RentCastLastUpdated { get; set; } // When data was last fetched

        // Merged from SellerListing
        public decimal? SuggestedPrice { get; set; }
        [MaxLength(2000)]
        public string? DocumentUrlsJson { get; set; }
        public string? ReviewStatus { get; set; } // NotStarted, CheckingDocuments, etc.
        public int? ReviewTimeframeDays { get; set; }
        public DateTime? ReviewDueDate { get; set; }
        public bool DocumentsVerified { get; set; } = false;
        public bool InspectionCompleted { get; set; } = false;
        public bool PropertyVerified { get; set; } = false;
        public bool SellerContacted { get; set; } = false;
        public bool ReadyForApproval { get; set; } = false;

        // Merged from ListingReview
        public int? ExpectedTimeframeDays { get; set; }
        public DateTime? ExpectedCompletionDate { get; set; }
        public bool DocumentsReceived { get; set; } = false;
        public DateTime? DocumentsReceivedAt { get; set; }
        public DateTime? DocumentsVerifiedAt { get; set; }
        [MaxLength(1000)]
        public string? DocumentNotes { get; set; }
        [MaxLength(1000)]
        public string? VerifiedDocumentTypesJson { get; set; }
        public bool InspectionScheduled { get; set; } = false;
        public DateTime? InspectionScheduledDate { get; set; }
        public DateTime? InspectionCompletedAt { get; set; }
        [MaxLength(1000)]
        public string? InspectionNotes { get; set; }
        [MaxLength(50)]
        public string? InspectionResult { get; set; }
        public bool AreaVerified { get; set; } = false;
        public bool LocationVerified { get; set; } = false;
        public bool AmenitiesVerified { get; set; } = false;
        [MaxLength(1000)]
        public string? PropertyDetailsNotes { get; set; }
        public DateTime? SellerContactedAt { get; set; }
        [MaxLength(100)]
        public string? ContactMethod { get; set; }
        [MaxLength(1000)]
        public string? ContactNotes { get; set; }
        public bool DealArrangementScheduled { get; set; } = false;
        public DateTime? DealArrangementDate { get; set; }
        public DateTime? ReadyForApprovalAt { get; set; }
        [MaxLength(1000)]
        public string? FinalReviewNotes { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;
        public DateTime? DecisionAt { get; set; }
        [MaxLength(1000)]
        public string? RejectionReason { get; set; }
        [MaxLength(2000)]
        public string? AuditLogJson { get; set; }

        // Manager Notes (from SellerListing/ListingReview)
        [MaxLength(1000)]
        public string? ManagerNotes { get; set; }

        // Status Label helper
        public string StatusLabel => Status;

        // Specs
        public int Sqft { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int ParkingSlots { get; set; }

        // Cover Image
        [MaxLength(500)]
        public string? CoverImage { get; set; }

        // ImageUrls helper for views (backward compatibility)
        public List<string> ImageUrls => !string.IsNullOrEmpty(CoverImage) ? new List<string> { CoverImage } : new List<string>();

        // Link to source SellerListing (for document access)
        public int? SourceSellerListingId { get; set; }

        // Tracking
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Sale Status
        public bool IsSold { get; set; } = false;
        public DateTime? SoldDate { get; set; }
        
        // Archive Status
        public bool IsArchived { get; set; } = false;
        public DateTime? ArchivedAt { get; set; }
        
        // Soft Delete Status
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        
        // Ready for Listing Status (after manager processes sale)
        public bool IsReadyForListing { get; set; } = false;
        public DateTime? ReadyForListingDate { get; set; }

        // Collections
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<PropertyInquiry> Inquiries { get; set; } = new List<PropertyInquiry>();
    }
}
