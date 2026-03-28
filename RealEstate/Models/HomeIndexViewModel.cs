namespace RealEstate.Models;

public class HomeIndexViewModel
{
    public IReadOnlyList<Property> FeaturedProperties { get; set; } = Array.Empty<Property>();
    public IReadOnlyList<PromotionItem> Promotions { get; set; } = Array.Empty<PromotionItem>();
}

public class PromotionItem
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
