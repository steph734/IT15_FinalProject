using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models
{
    public class SystemSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public string SiteName { get; set; } = "EstateFlow";

        [MaxLength(500)]
        public string SiteDescription { get; set; } = string.Empty;

        [MaxLength(255)]
        public string ContactEmail { get; set; } = string.Empty;

        [MaxLength(50)]
        public string SupportPhone { get; set; } = string.Empty;

        public decimal DefaultCommissionRate { get; set; } = 5.0m;

        public decimal MinimumPropertyPrice { get; set; } = 10000m;

        public bool EnableRegistration { get; set; } = true;

        public bool MaintenanceMode { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
