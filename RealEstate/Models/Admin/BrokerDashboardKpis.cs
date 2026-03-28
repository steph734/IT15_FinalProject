namespace RealEstate.Models.Admin;

public class BrokerDashboardKpis
{
    public int TotalDealsClosed { get; set; }

    public decimal TotalSalesValue { get; set; }

    public decimal TotalCommissionEarned { get; set; }

    public int ActiveDeals { get; set; }
}

public class BrokerChartSeries
{
    public IReadOnlyList<string> MonthlyLabels { get; init; } = Array.Empty<string>();

    public IReadOnlyList<decimal> MonthlySalesValues { get; init; } = Array.Empty<decimal>();

    public IReadOnlyList<decimal> MonthlyCommissions { get; init; } = Array.Empty<decimal>();

    public IReadOnlyList<int> MonthlyClosedCounts { get; init; } = Array.Empty<int>();

    public IReadOnlyList<string> StatusLabels { get; init; } = Array.Empty<string>();

    public IReadOnlyList<int> StatusCounts { get; init; } = Array.Empty<int>();
}
