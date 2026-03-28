using RealEstate.Models;

namespace RealEstate.Services;

public static class TransactionChargeCalculator
{
    /// <summary>
    /// Buy: earnest money (1% of list price, minimum ₱25,000).
    /// Rent: advance (1 month) + security deposit (2 months), common PH practice for demo.
    /// </summary>
    public static TransactionChargeSummary Summarize(Property property)
    {
        if (property.ListingType == PropertyListingType.Buy)
        {
            var earnest = Math.Max(25_000m, Math.Round(property.Price * 0.01m, 0));
            return new TransactionChargeSummary
            {
                TotalDue = earnest,
                SummaryLine = "Earnest money (1% of list price, minimum ₱25,000)",
                BuyEarnestMoney = earnest,
                RentAdvance = null,
                RentSecurityDeposit = null
            };
        }

        var advance = property.Price;
        var security = property.Price * 2m;
        return new TransactionChargeSummary
        {
            TotalDue = advance + security,
            SummaryLine = "Advance rent (1 month) + security deposit (2 months)",
            RentAdvance = advance,
            RentSecurityDeposit = security,
            BuyEarnestMoney = null
        };
    }
}

public class TransactionChargeSummary
{
    public decimal TotalDue { get; set; }
    public string SummaryLine { get; set; } = string.Empty;
    public decimal? BuyEarnestMoney { get; set; }
    public decimal? RentAdvance { get; set; }
    public decimal? RentSecurityDeposit { get; set; }
}
