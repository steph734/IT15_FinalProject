namespace RealEstate.Models;

public class TransactionPageViewModel
{
    public bool ShowSystemApproved { get; set; }

    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string PropertyLocation { get; set; } = string.Empty;
    public PropertyListingType ListingType { get; set; }
    public decimal ListPrice { get; set; }
    public string ChargeSummaryLine { get; set; } = string.Empty;
    public decimal TotalDue { get; set; }
    public decimal? RentAdvance { get; set; }
    public decimal? RentSecurityDeposit { get; set; }
    public decimal? BuyEarnestMoney { get; set; }

    public TransactionFormModel Form { get; set; } = new();
}
