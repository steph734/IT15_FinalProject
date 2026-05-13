using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models
{
    // Moving this here ensures other classes can see "AppointmentStatus"
    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }

    public class ViewingAppointment
    {
        [Key]
        public int Id { get; set; }

        // --- NEW: This fixes the 'AppointmentId' definition error ---
        [Required]
        public int AppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
        // -------------------------------------------------------------

        public int PropertyId { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; }

        public int? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? CustomerEmail { get; set; }

        [StringLength(20)]
        public string? CustomerPhone { get; set; }

        [StringLength(500)]
        public string? CustomerPhotoUrl { get; set; }

        [Required]
        public DateTime WhenUtc { get; set; }

        [StringLength(20)]
        public string? PreferredTime { get; set; }

        public int NumberOfVisitors { get; set; } = 1;

        [StringLength(50)]
        public string? BuyerType { get; set; }

        [StringLength(50)]
        public string? FinancingStatus { get; set; }

        [StringLength(50)]
        public string? InformationSource { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    }
}