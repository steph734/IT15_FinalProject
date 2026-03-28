using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Helpers;
using RealEstate.Models.Admin;
using RealEstate.Services;

namespace RealEstate.Controllers.Admin;

[Route("admin/commissions")]
public class AdminCommissionsController : AdminBaseController
{
    private readonly BrokerDealLedger _ledger;
    private readonly PropertyCatalog _catalog;

    public AdminCommissionsController(BrokerDealLedger ledger, PropertyCatalog catalog)
    {
        _ledger = ledger;
        _catalog = catalog;
    }

    [HttpGet]
    [Route("")]
    public IActionResult Index()
    {
        if (!IsBrokerPortalSignedIn())
            return BrokerPortalLoginRedirect();

        var deals = _ledger.GetAll().OrderByDescending(d => d.DealDate).ToList();
        var totalEarned = deals
            .Where(d => d.Status == BrokerDealStatus.Completed)
            .Sum(d => d.CommissionAmount);

        var rows = deals.Select(d =>
        {
            var prop = _catalog.GetProperty(d.PropertyId);
            return new BrokerCommissionRowModel
            {
                TransactionId = d.TransactionId,
                PropertyTitle = prop?.Title ?? $"Property #{d.PropertyId}",
                ClientName = d.ClientName,
                TypeDisplay = d.DealType == BrokerDealType.Sale ? "Sale" : "Rent",
                DealAmount = d.Amount,
                DealAmountFormatted = d.Amount.ToString("N0", Currency.PhilippinePeso),
                CommissionAmount = d.CommissionAmount,
                CommissionFormatted = d.CommissionAmount.ToString("N0", Currency.PhilippinePeso),
                CommissionPaid = d.CommissionPaid,
                PaymentDisplay = d.CommissionPaid == BrokerCommissionPaidStatus.Paid ? "Paid" : "Unpaid",
                DealDateUtc = d.DealDate,
                DateDisplay = d.DealDate.ToLocalTime().ToString("MMM d, yyyy", CultureInfo.InvariantCulture)
            };
        }).ToList();

        var vm = new AdminCommissionsIndexViewModel
        {
            TotalCommissionEarned = totalEarned,
            TotalCommissionFormatted = totalEarned.ToString("N0", Currency.PhilippinePeso),
            Rows = rows
        };

        return View("~/Views/Admin/Commissions/Index.cshtml", vm);
    }
}
