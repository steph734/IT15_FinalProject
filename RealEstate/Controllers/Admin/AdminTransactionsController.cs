using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Helpers;
using RealEstate.Models.Admin;
using RealEstate.Services;

namespace RealEstate.Controllers.Admin;

[Route("admin/transactions")]
public class AdminTransactionsController : AdminBaseController
{
    private readonly BrokerDealLedger _ledger;
    private readonly PropertyCatalog _catalog;

    public AdminTransactionsController(BrokerDealLedger ledger, PropertyCatalog catalog)
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

        var rows = _ledger.GetAll()
            .OrderByDescending(d => d.DealDate)
            .Select(MapRow)
            .ToList();

        return View("~/Views/Admin/Transactions/Index.cshtml", new AdminTransactionsIndexViewModel { Rows = rows });
    }

    [HttpGet("{id}")]
    public IActionResult Details(string id)
    {
        if (!IsBrokerPortalSignedIn())
            return BrokerPortalLoginRedirect();

        var deal = _ledger.GetById(id);
        if (deal is null)
            return NotFound();

        var property = _catalog.GetProperty(deal.PropertyId);
        var vm = new AdminTransactionDetailsViewModel { Deal = deal, Listing = property };
        return View("~/Views/Admin/Transactions/Details.cshtml", vm);
    }

    private BrokerTransactionRowModel MapRow(BrokerDeal d)
    {
        var prop = _catalog.GetProperty(d.PropertyId);
        return new BrokerTransactionRowModel
        {
            TransactionId = d.TransactionId,
            PropertyTitle = prop?.Title ?? $"Property #{d.PropertyId}",
            ClientName = d.ClientName,
            DealType = d.DealType,
            TypeDisplay = d.DealType == BrokerDealType.Sale ? "Sale" : "Rent",
            Amount = d.Amount,
            AmountFormatted = d.Amount.ToString("N0", Currency.PhilippinePeso),
            Status = d.Status,
            StatusDisplay = d.Status == BrokerDealStatus.Completed ? "Completed" : "Pending",
            DealDateUtc = d.DealDate,
            DateDisplay = d.DealDate.ToLocalTime().ToString("MMM d, yyyy", CultureInfo.InvariantCulture)
        };
    }
}
