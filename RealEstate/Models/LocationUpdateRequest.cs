using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class LocationUpdateRequest
    {
        [Required]
        public int AgentId { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public string? Address { get; set; }

        public int? AppointmentId { get; set; }
    }
}
