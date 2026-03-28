using RealEstate.Models;
using RealEstate.Models.Admin;

namespace RealEstate.Services;

public class BrokerDealLedger
{
    private readonly IReadOnlyList<BrokerDeal> _deals;

    public BrokerDealLedger()
    {
        _deals =
        [
            new BrokerDeal
            {
                TransactionId = "EFL-20260108-A1B2C3D4",
                PropertyId = 2,
                ClientName = "Ana Patricia Lim",
                ClientEmail = "ana.lim@email.ph",
                ClientPhone = "+63 917 555 0101",
                DealType = BrokerDealType.Sale,
                Amount = 10_500_000m,
                Status = BrokerDealStatus.Completed,
                DealDate = new DateTime(2026, 1, 8, 14, 30, 0, DateTimeKind.Utc),
                PaymentStatus = BrokerPaymentStatus.Paid,
                CommissionRate = 0.025m,
                CommissionAmount = 262_500m,
                CommissionPaid = BrokerCommissionPaidStatus.Paid,
                PipelineStage = BrokerPipelineStage.Closed,
                CommissionNotes = "2.5% cooperative broker split on list price."
            },
            new BrokerDeal
            {
                TransactionId = "EFL-20260115-E5F6A7B8",
                PropertyId = 6,
                ClientName = "Roberto Villanueva",
                ClientEmail = "rvillanueva@corp.ph",
                ClientPhone = "+63 918 222 8844",
                DealType = BrokerDealType.Rent,
                Amount = 72_000m * 12,
                Status = BrokerDealStatus.Completed,
                DealDate = new DateTime(2026, 1, 15, 9, 0, 0, DateTimeKind.Utc),
                PaymentStatus = BrokerPaymentStatus.Paid,
                CommissionRate = 0.5m,
                CommissionAmount = 36_000m,
                CommissionPaid = BrokerCommissionPaidStatus.Paid,
                PipelineStage = BrokerPipelineStage.Closed,
                CommissionNotes = "50% of first month’s rent (leasing desk policy)."
            },
            new BrokerDeal
            {
                TransactionId = "EFL-20260122-C9D0E1F2",
                PropertyId = 1,
                ClientName = "David & Christine Wu",
                ClientEmail = "dwu.family@outlook.com",
                ClientPhone = "+63 919 333 1200",
                DealType = BrokerDealType.Sale,
                Amount = 24_500_000m,
                Status = BrokerDealStatus.Pending,
                DealDate = new DateTime(2026, 1, 22, 11, 15, 0, DateTimeKind.Utc),
                PaymentStatus = BrokerPaymentStatus.Pending,
                CommissionRate = 0.025m,
                CommissionAmount = 612_500m,
                CommissionPaid = BrokerCommissionPaidStatus.Unpaid,
                PipelineStage = BrokerPipelineStage.Negotiation,
                CommissionNotes = "2.5% gross; escrow pending bank release."
            },
            new BrokerDeal
            {
                TransactionId = "EFL-20260201-11223344",
                PropertyId = 5,
                ClientName = "Francis De Guzman",
                ClientEmail = "fguzman@startup.ph",
                ClientPhone = "+63 917 444 9090",
                DealType = BrokerDealType.Sale,
                Amount = 18_900_000m,
                Status = BrokerDealStatus.Completed,
                DealDate = new DateTime(2026, 2, 1, 16, 45, 0, DateTimeKind.Utc),
                PaymentStatus = BrokerPaymentStatus.Paid,
                CommissionRate = 0.025m,
                CommissionAmount = 472_500m,
                CommissionPaid = BrokerCommissionPaidStatus.Paid,
                PipelineStage = BrokerPipelineStage.Closed,
                CommissionNotes = "Standard seller-paid commission."
            },
            new BrokerDeal
            {
                TransactionId = "EFL-20260210-55667788",
                PropertyId = 7,
                ClientName = "Michelle Reyes",
                ClientEmail = "mreyes.ph@gmail.com",
                ClientPhone = "+63 916 121 3434",
                DealType = BrokerDealType.Rent,
                Amount = 22_000m * 12,
                Status = BrokerDealStatus.Completed,
                DealDate = new DateTime(2026, 2, 10, 8, 0, 0, DateTimeKind.Utc),
                PaymentStatus = BrokerPaymentStatus.Paid,
                CommissionRate = 0.5m,
                CommissionAmount = 11_000m,
                CommissionPaid = BrokerCommissionPaidStatus.Unpaid,
                PipelineStage = BrokerPipelineStage.Closed,
                CommissionNotes = "First month split; landlord payout scheduled."
            },
            new BrokerDeal
            {
                TransactionId = "EFL-20260218-99AABBCC",
                PropertyId = 4,
                ClientName = "Cebu Workspace Inc.",
                ClientEmail = "leases@cebuworkspace.ph",
                ClientPhone = "+63 32 255 9000",
                DealType = BrokerDealType.Sale,
                Amount = 6_200_000m,
                Status = BrokerDealStatus.Pending,
                DealDate = new DateTime(2026, 2, 18, 13, 20, 0, DateTimeKind.Utc),
                PaymentStatus = BrokerPaymentStatus.Pending,
                CommissionRate = 0.025m,
                CommissionAmount = 155_000m,
                CommissionPaid = BrokerCommissionPaidStatus.Unpaid,
                PipelineStage = BrokerPipelineStage.Viewing,
                CommissionNotes = "Investor purchase; DOC review in progress."
            },
            new BrokerDeal
            {
                TransactionId = "EFL-20260228-DDEEFF00",
                PropertyId = 9,
                ClientName = "Alvarez Family Trust",
                ClientEmail = "trust@alvarez.ph",
                ClientPhone = "+63 2 8812 7700",
                DealType = BrokerDealType.Sale,
                Amount = 185_000_000m,
                Status = BrokerDealStatus.Completed,
                DealDate = new DateTime(2026, 2, 28, 10, 0, 0, DateTimeKind.Utc),
                PaymentStatus = BrokerPaymentStatus.Paid,
                CommissionRate = 0.02m,
                CommissionAmount = 3_700_000m,
                CommissionPaid = BrokerCommissionPaidStatus.Paid,
                PipelineStage = BrokerPipelineStage.Closed,
                CommissionNotes = "2% negotiated on ultra-luxury lane."
            },
            new BrokerDeal
            {
                TransactionId = "EFL-20260305-13572468",
                PropertyId = 3,
                ClientName = "Lori Ann Makisig",
                ClientEmail = "lori.makisig@yahoo.com",
                ClientPhone = "+63 917 888 4411",
                DealType = BrokerDealType.Sale,
                Amount = 12_800_000m,
                Status = BrokerDealStatus.Pending,
                DealDate = new DateTime(2026, 3, 5, 15, 30, 0, DateTimeKind.Utc),
                PaymentStatus = BrokerPaymentStatus.Pending,
                CommissionRate = 0.025m,
                CommissionAmount = 320_000m,
                CommissionPaid = BrokerCommissionPaidStatus.Unpaid,
                PipelineStage = BrokerPipelineStage.Inquiry,
                CommissionNotes = "Pipeline: inquiry → viewing scheduled."
            },
            new BrokerDeal
            {
                TransactionId = "EFL-20260312-24681357",
                PropertyId = 8,
                ClientName = "Janine Salazar",
                ClientEmail = "janine.s@uni.edu.ph",
                ClientPhone = "+63 915 600 7788",
                DealType = BrokerDealType.Rent,
                Amount = 14_500m * 12,
                Status = BrokerDealStatus.Completed,
                DealDate = new DateTime(2026, 3, 12, 9, 45, 0, DateTimeKind.Utc),
                PaymentStatus = BrokerPaymentStatus.Paid,
                CommissionRate = 0.5m,
                CommissionAmount = 7_250m,
                CommissionPaid = BrokerCommissionPaidStatus.Paid,
                PipelineStage = BrokerPipelineStage.Closed,
                CommissionNotes = "Student housing lease; commission released."
            },
            new BrokerDeal
            {
                TransactionId = "EFL-20260320-ABCDEF01",
                PropertyId = 1,
                ClientName = "K + R Holdings",
                ClientEmail = "ops@krholdings.ph",
                ClientPhone = "+63 2 8800 1200",
                DealType = BrokerDealType.Sale,
                Amount = 24_500_000m,
                Status = BrokerDealStatus.Pending,
                DealDate = new DateTime(2026, 3, 20, 12, 0, 0, DateTimeKind.Utc),
                PaymentStatus = BrokerPaymentStatus.Pending,
                CommissionRate = 0.025m,
                CommissionAmount = 612_500m,
                CommissionPaid = BrokerCommissionPaidStatus.Unpaid,
                PipelineStage = BrokerPipelineStage.Negotiation,
                CommissionNotes = "Competing offer countered; awaiting seller sign-off."
            }
        ];
    }

    public BrokerDashboardKpis GetKpis()
    {
        var completed = _deals.Where(d => d.Status == BrokerDealStatus.Completed).ToList();
        var closedSalesValue = completed
            .Where(d => d.DealType == BrokerDealType.Sale)
            .Sum(d => d.Amount);
        var commissionEarned = completed.Sum(d => d.CommissionAmount);
        return new BrokerDashboardKpis
        {
            TotalDealsClosed = completed.Count,
            TotalSalesValue = closedSalesValue,
            TotalCommissionEarned = commissionEarned,
            ActiveDeals = _deals.Count(d => d.Status == BrokerDealStatus.Pending)
        };
    }

    /// <summary>
    /// Monthly buckets in UTC for charts (last completed deals by DealDate).
    /// </summary>
    public BrokerChartSeries GetChartSeries(int monthCount = 6)
    {
        monthCount = Math.Clamp(monthCount, 3, 12);
        var now = DateTime.UtcNow;
        var start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-(monthCount - 1));

        var completed = _deals.Where(d => d.Status == BrokerDealStatus.Completed).ToList();
        var labels = new List<string>();
        var salesValues = new List<decimal>();
        var commissions = new List<decimal>();
        var closedCounts = new List<int>();

        for (var m = 0; m < monthCount; m++)
        {
            var monthStart = start.AddMonths(m);
            var monthEnd = monthStart.AddMonths(1);
            labels.Add(monthStart.ToString("MMM yyyy"));
            var inMonth = completed.Where(d => d.DealDate >= monthStart && d.DealDate < monthEnd).ToList();
            closedCounts.Add(inMonth.Count);
            salesValues.Add(inMonth.Where(d => d.DealType == BrokerDealType.Sale).Sum(d => d.Amount));
            commissions.Add(inMonth.Sum(d => d.CommissionAmount));
        }

        var pending = _deals.Count(d => d.Status == BrokerDealStatus.Pending);
        var done = _deals.Count(d => d.Status == BrokerDealStatus.Completed);

        return new BrokerChartSeries
        {
            MonthlyLabels = labels,
            MonthlySalesValues = salesValues,
            MonthlyCommissions = commissions,
            MonthlyClosedCounts = closedCounts,
            StatusLabels = new[] { "Pending", "Completed" },
            StatusCounts = new[] { pending, done }
        };
    }

    public IReadOnlyList<BrokerDeal> GetAll() => _deals;

    public BrokerDeal? GetById(string transactionId) =>
        _deals.FirstOrDefault(d => string.Equals(d.TransactionId, transactionId, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<BrokerDeal> GetCompleted() =>
        _deals.Where(d => d.Status == BrokerDealStatus.Completed).OrderByDescending(d => d.DealDate).ToList();

    public IReadOnlyList<BrokerDeal> FilterSalesHistory(DateTime? fromUtc, DateTime? toUtc, int? propertyId, string? client)
    {
        IEnumerable<BrokerDeal> q = _deals.Where(d => d.Status == BrokerDealStatus.Completed);
        if (fromUtc is { } f)
            q = q.Where(d => d.DealDate >= f.Date);
        if (toUtc is { } t)
        {
            var endExclusive = t.Date.AddDays(1);
            q = q.Where(d => d.DealDate < endExclusive);
        }
        if (propertyId is { } pid)
            q = q.Where(d => d.PropertyId == pid);
        if (!string.IsNullOrWhiteSpace(client))
        {
            var c = client.Trim();
            q = q.Where(d => d.ClientName.Contains(c, StringComparison.OrdinalIgnoreCase));
        }

        return q.OrderByDescending(d => d.DealDate).ToList();
    }
}
