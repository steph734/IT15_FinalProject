using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Helpers;
using RealEstate.Models.Admin;
using RealEstate.Services;

namespace RealEstate.Controllers.Admin;

[Route("admin/sales-history")]
public class AdminSalesHistoryController : AdminBaseController
{
    private readonly BrokerDealLedger _ledger;
    private readonly PropertyCatalog _catalog;

    public AdminSalesHistoryController(BrokerDealLedger ledger, PropertyCatalog catalog)
    {
        _ledger = ledger;
        _catalog = catalog;
    }

    [HttpGet]
    [Route("")]
    public IActionResult Index(DateTime? from, DateTime? to, int? propertyId, string? client)
    {
        if (!IsBrokerPortalSignedIn())
            return BrokerPortalLoginRedirect();

        DateTime? fromUtc = from.HasValue
            ? DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc)
            : null;
        DateTime? toUtc = to.HasValue
            ? DateTime.SpecifyKind(to.Value.Date, DateTimeKind.Utc)
            : null;

        var filtered = _ledger.FilterSalesHistory(fromUtc, toUtc, propertyId, client);

        var rows = filtered.Select(d =>
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
                StatusDisplay = "Completed",
                DealDateUtc = d.DealDate,
                DateDisplay = d.DealDate.ToLocalTime().ToString("MMM d, yyyy", CultureInfo.InvariantCulture)
            };
        }).ToList();

        var propertyOptions = _catalog.GetProperties()
            .OrderBy(p => p.Title)
            .Select(p => new PropertyPickItem { Id = p.Id, Title = p.Title })
            .ToList();

        var vm = new AdminSalesHistoryViewModel
        {
            Rows = rows,
            PropertyOptions = propertyOptions,
            FilterFromUtc = fromUtc,
            FilterToUtc = toUtc,
            FilterPropertyId = propertyId,
            FilterClient = client
        };

        return View("~/Views/Admin/SalesHistory/Index.cshtml", vm);
    }
}
