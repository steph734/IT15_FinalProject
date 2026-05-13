using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class Otp
    {
        [Key]
        public int OtpId { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string OtpCode { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
