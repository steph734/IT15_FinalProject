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
    // Current status managed by broker
    public PropertyStatus Status { get; set; } = PropertyStatus.AvailableForSale;

    // Accommodation details
    /// <summary>Number of bedrooms.</summary>
    public int Bedrooms { get; set; } = 1;

    /// <summary>Number of bathrooms.</summary>
    public int Bathrooms { get; set; } = 1;

    /// <summary>Number of parking slots (0 when none).</summary>
    public int ParkingSlots { get; set; } = 0;
}

public enum PropertyStatus
{
    AvailableForSale,
    AvailableForRent,
    Reserved,
    Sold,
    Rented
}
