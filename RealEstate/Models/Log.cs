using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }
    }
}
