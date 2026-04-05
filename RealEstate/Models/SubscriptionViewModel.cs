using RealEstate.Services;

namespace RealEstate.Models;

public class SubscriptionViewModel
{
    public SubscriptionTier CurrentTier { get; set; }

    // Prices for display (monthly)
    public decimal FreemiumPrice { get; set; } = 0m;
    public decimal PlusPrice { get; set; } = 29m;
    public decimal PremiumPrice { get; set; } = 59m;

    public bool Yearly { get; set; }
}
