using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        public Property Property { get; set; } = null!;

        public int? EmployeeId { get; set; }

        public int? AgentId { get; set; }
        [ForeignKey("AgentId")]
        public Agent? Agent { get; set; }

        public int? ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public Schedule? Schedule { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Scheduled";

        [StringLength(100)]
        public string? MeetingPoint { get; set; }

        // Customer Information from Viewing Form
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;

        [StringLength(20)]
        public string? CustomerPhone { get; set; }

        [StringLength(500)]
        public string? CustomerPhotoUrl { get; set; }

        // Viewing Details
        public int NumberOfVisitors { get; set; } = 1;

        [StringLength(50)]
        public string? BuyerType { get; set; }

        [StringLength(50)]
        public string? FinancingStatus { get; set; }

        [StringLength(100)]
        public string? InformationSource { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Schedule Information (denormalized for quick access)
        [Required]
        public DateTime PreferredDate { get; set; }

        [StringLength(100)]
        public string? PreferredTime { get; set; }

        // Appointment Type (self or pickup)
        [StringLength(20)]
        public string? AppointmentType { get; set; } = "self";

        // Customer Address (for pickup option)
        [StringLength(200)]
        public string? CustomerAddress { get; set; }

        // Customer Location Coordinates (for pickup option)
        public double? CustomerLatitude { get; set; }
        public double? CustomerLongitude { get; set; }

        // Broker Location Coordinates (for real-time tracking)
        public double? BrokerLatitude { get; set; }
        public double? BrokerLongitude { get; set; }
        public DateTime? BrokerLocationLastUpdated { get; set; }

        // ID Information (for self-visit)
        [StringLength(50)]
        public string? IdType { get; set; } = null;

        [StringLength(100)]
        public string? IdNumber { get; set; } = null;

        // Transportation Information (for self-visit)
        [StringLength(50)]
        public string? TransportationMethod { get; set; } = null;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}