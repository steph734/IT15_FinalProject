namespace RealEstate.Models;

public class Property
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    /// <summary>Floor area in square meters (sqm), typical for PH listings.</summary>
    public int Sqft { get; set; }
    /// <summary>Price in Philippine pesos (PHP): total for sale, monthly for rent.</summary>
    public decimal Price { get; set; }
    public PropertyListingType ListingType { get; set; }
    public string Description { get; set; } = string.Empty;
    public IReadOnlyList<string> ImageUrls { get; set; } = Array.Empty<string>();
    public int AgentId { get; set; }
}
