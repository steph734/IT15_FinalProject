using System.ComponentModel.DataAnnotations;
using RealEstate.Models;

namespace RealEstate.ViewModels
{
    public class ScheduleVisitViewModel
    {
        [Required]
        public int PropertyId { get; set; }

        public Property? Property { get; set; }

        [Required(ErrorMessage = "Please select a broker")]
        public int BrokerId { get; set; }

        public List<BrokerViewModel> AvailableBrokers { get; set; } = new();

        [Required(ErrorMessage = "Please select a date")]
        [DataType(DataType.Date)]
        [Display(Name = "Visit Date")]
        public DateTime ScheduledDate { get; set; }

        [Required(ErrorMessage = "Please select a time slot")]
        [Display(Name = "Time Slot")]
        public TimeSpan ScheduledTime { get; set; }

        public List<TimeSpan> AvailableTimeSlots { get; set; } = new();

        [Display(Name = "Pickup Service")]
        public bool PickupService { get; set; } = false;

        [StringLength(500)]
        [Display(Name = "Pickup Address")]
        public string? PickupAddress { get; set; }

        [Display(Name = "Pickup Latitude")]
        public double? PickupLatitude { get; set; }

        [Display(Name = "Pickup Longitude")]
        public double? PickupLongitude { get; set; }

        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }

        [StringLength(1000)]
        [Display(Name = "Special Requests")]
        public string? ClientNotes { get; set; }
    }

    public class BrokerViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ProfileImageUrl { get; set; }
        public double? Rating { get; set; }
        public int CompletedVisits { get; set; }
    }

    public class VisitTrackingViewModel
    {
        public int VisitId { get; set; }
        public Property Property { get; set; } = null!;
        public ApplicationUser Client { get; set; } = null!;
        public ApplicationUser Broker { get; set; } = null!;
        public string Status { get; set; } = "Pending";
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public bool PickupService { get; set; }
        public string? PickupAddress { get; set; }
        public double? PickupLatitude { get; set; }
        public double? PickupLongitude { get; set; }
        public double? CurrentBrokerLatitude { get; set; }
        public double? CurrentBrokerLongitude { get; set; }
        public DateTime? LastGpsUpdate { get; set; }
        public int? EstimatedArrivalMinutes { get; set; }
        public bool IsClient { get; set; }
        public bool IsBroker { get; set; }
    }

    public class VisitFeedbackViewModel
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Please rate the property from 1 to 5 stars")]
        [Display(Name = "Property Rating")]
        public int PropertyRating { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Please rate the broker's service from 1 to 5 stars")]
        [Display(Name = "Broker Service Rating")]
        public int BrokerServiceRating { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Please rate the pickup experience from 1 to 5 stars")]
        [Display(Name = "Pickup Experience Rating")]
        public int PickupExperienceRating { get; set; }

        [StringLength(1000)]
        [Display(Name = "Additional Feedback")]
        public string? ClientFeedback { get; set; }
    }
}
