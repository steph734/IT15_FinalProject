using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class PropertySeller
    {
        [Key]
        public int SellerId { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string SellerName { get; set; }
        public int Rating { get; set; }
        public bool IdentityVerified { get; set; }
        public ICollection<Property> Properties { get; set; }
    }
}
