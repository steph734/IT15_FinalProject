using RealEstate.Models;

namespace RealEstate.Models.Admin;

public class AdminTransactionsIndexViewModel
{
    public IReadOnlyList<BrokerTransactionRowModel> Rows { get; init; } = Array.Empty<BrokerTransactionRowModel>();
}

public class BrokerTransactionRowModel
{
    public string TransactionId { get; init; } = "";

    public string PropertyTitle { get; init; } = "";

    public string ClientName { get; init; } = "";

    public BrokerDealType DealType { get; init; }

    public string TypeDisplay { get; init; } = "";

    public decimal Amount { get; init; }

    public string AmountFormatted { get; init; } = "";

    public BrokerDealStatus Status { get; init; }

    public string StatusDisplay { get; init; } = "";

    public DateTime DealDateUtc { get; init; }

    public string DateDisplay { get; init; } = "";
}

public class AdminTransactionDetailsViewModel
{
    public BrokerDeal Deal { get; init; } = null!;

    public Property? Listing { get; init; }
}

public class AdminCommissionsIndexViewModel
{
    public decimal TotalCommissionEarned { get; init; }

    public string TotalCommissionFormatted { get; init; } = "";

    public IReadOnlyList<BrokerCommissionRowModel> Rows { get; init; } = Array.Empty<BrokerCommissionRowModel>();
}

public class BrokerCommissionRowModel
{
    public string TransactionId { get; init; } = "";

    public string PropertyTitle { get; init; } = "";

    public string ClientName { get; init; } = "";

    public string TypeDisplay { get; init; } = "";

    public decimal DealAmount { get; init; }

    public string DealAmountFormatted { get; init; } = "";

    public decimal CommissionAmount { get; init; }

    public string CommissionFormatted { get; init; } = "";

    public string PaymentDisplay { get; init; } = "";

    public BrokerCommissionPaidStatus CommissionPaid { get; init; }

    public DateTime DealDateUtc { get; init; }

    public string DateDisplay { get; init; } = "";
}

public class PropertyPickItem
{
    public int Id { get; init; }

    public string Title { get; init; } = "";
}

public class AdminSalesHistoryViewModel
{
    public IReadOnlyList<BrokerTransactionRowModel> Rows { get; init; } = Array.Empty<BrokerTransactionRowModel>();

    public IReadOnlyList<PropertyPickItem> PropertyOptions { get; init; } = Array.Empty<PropertyPickItem>();

    public DateTime? FilterFromUtc { get; init; }

    public DateTime? FilterToUtc { get; init; }

    public int? FilterPropertyId { get; init; }

    public string? FilterClient { get; init; }
}
