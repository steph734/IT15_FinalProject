using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class SelfVisitRequest
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PreferredDate { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PreferredTime { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string IdType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string IdNumber { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Transportation { get; set; }

        [StringLength(200)]
        public string? CustomerAddress { get; set; }

        public double? CustomerLatitude { get; set; }
        public double? CustomerLongitude { get; set; }
    }
}
