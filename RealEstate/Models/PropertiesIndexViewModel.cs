namespace RealEstate.Models;

public class PropertiesIndexViewModel
{
    public IReadOnlyList<Property> Properties { get; set; } = Array.Empty<Property>();

    public string? Location { get; set; }
    public string? PriceRange { get; set; }

    /// <summary>Location filter values (matched as substring against listing address).</summary>
    public static IReadOnlyList<(string Value, string Label)> SearchLocations { get; } =
    [
        ("", "Any"),
        ("Taguig", "BGC, Taguig"),
        ("Makati", "Makati"),
        ("Quezon City", "Quezon City"),
        ("Cebu", "Cebu"),
        ("Davao", "Davao City"),
        ("Pasig", "Ortigas, Pasig")
    ];

    /// <summary>
    /// Price tiers: for Buy, total PHP; for Rent, monthly PHP.
    /// Keys p1–p4 align with <see cref="RealEstate.Services.PropertyCatalog"/> filtering.
    /// </summary>
    public static IReadOnlyDictionary<string, string> PriceRangeLabels { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["any"] = "Any price",
            ["p1"] = "Up to ₱8M · rent up to ₱25k",
            ["p2"] = "₱8M–15M · rent ₱25–45k",
            ["p3"] = "₱15M–30M · rent ₱45–75k",
            ["p4"] = "₱30M+ · rent ₱75k+"
        };
}
