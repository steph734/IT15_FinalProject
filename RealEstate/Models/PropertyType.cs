using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class PropertyType
    {
        [Key]
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public ICollection<Property> Properties { get; set; }
    }
}
