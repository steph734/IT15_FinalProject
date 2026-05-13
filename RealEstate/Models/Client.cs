using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        // This links the client to a specific Branch (Davao, Panabo, Tagum)
        public int BranchId { get; set; }
    }
}
