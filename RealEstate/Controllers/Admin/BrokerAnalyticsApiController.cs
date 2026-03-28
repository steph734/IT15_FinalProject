using Microsoft.AspNetCore.Mvc;
using RealEstate.Models.Admin;
using RealEstate.Services;

namespace RealEstate.Controllers.Admin;

[Route("admin/api/broker")]
[ApiController]
public class BrokerAnalyticsApiController : ControllerBase
{
    private readonly BrokerDealLedger _ledger;

    public BrokerAnalyticsApiController(BrokerDealLedger ledger)
    {
        _ledger = ledger;
    }

    /// <summary>JSON for the broker dashboard (KPIs + chart datasets). Requires broker/admin session cookie.</summary>
    [HttpGet("dashboard")]
    [Produces("application/json")]
    public IActionResult GetDashboard()
    {
        if (HttpContext.Session.GetString(AdminSessionKeys.SignedIn) != "1")
            return Unauthorized();

        var kpis = _ledger.GetKpis();
        var chart = _ledger.GetChartSeries(6);

        return Ok(new
        {
            kpis = new
            {
                totalDealsClosed = kpis.TotalDealsClosed,
                totalSalesValue = kpis.TotalSalesValue,
                totalCommissionEarned = kpis.TotalCommissionEarned,
                activeDeals = kpis.ActiveDeals
            },
            charts = new
            {
                monthlyLabels = chart.MonthlyLabels,
                monthlySalesValues = chart.MonthlySalesValues.Select(d => (double)d).ToList(),
                monthlyCommissions = chart.MonthlyCommissions.Select(d => (double)d).ToList(),
                monthlyClosedCounts = chart.MonthlyClosedCounts,
                statusLabels = chart.StatusLabels,
                statusCounts = chart.StatusCounts
            }
        });
    }
}
