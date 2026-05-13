namespace RealEstate.Models.Seller;

public class SellerDashboardViewModel
{
    public int TotalListings { get; set; }
    public int PendingListings { get; set; }
    public int ApprovedListings { get; set; }
    public int SoldListings { get; set; }
    public int RejectedListings { get; set; }
    public decimal TotalEarnings { get; set; }
    public IReadOnlyList<Property> RecentListings { get; set; } = Array.Empty<Property>();
}
