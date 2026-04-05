namespace RealEstate.Models;

public class HomeIndexViewModel
{
    public IReadOnlyList<Property> FeaturedProperties { get; set; } = Array.Empty<Property>();
    public IReadOnlyList<PromotionItem> Promotions { get; set; } = Array.Empty<PromotionItem>();
    // Search results shown on the marketplace
    public IReadOnlyList<Property> SearchResults { get; set; } = Array.Empty<Property>();

    // Trending areas to highlight on the homepage
    public IReadOnlyList<string> TrendingAreas { get; set; } = Array.Empty<string>();

    // Echo of the current search filters
    public string? SearchLocation { get; set; }
    public decimal? SearchMaxPrice { get; set; }
    public int? SearchBedrooms { get; set; }
    public int? SearchRadiusKm { get; set; }
}

public class PromotionItem
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
