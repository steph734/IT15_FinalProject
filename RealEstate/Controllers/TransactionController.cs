using Microsoft.AspNetCore.Mvc;
using RealEstate.Models;
using RealEstate.Services;

namespace RealEstate.Controllers;

public class TransactionController : Controller
{
    private readonly PropertyCatalog _catalog;

    public TransactionController(PropertyCatalog catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    public IActionResult Index(int propertyId)
    {
        var property = _catalog.GetProperty(propertyId);
        if (property is null)
            return NotFound();

        var sessionPid = HttpContext.Session.GetString(TransactionSessionKeys.PropertyId);
        if (sessionPid != propertyId.ToString())
        {
            TempData["TransactionError"] =
                "Please submit a viewing request for this property first. After approval, you can complete your transaction here.";
            return RedirectToAction(nameof(PropertiesController.Details), "Properties", new { id = propertyId });
        }

        var charges = TransactionChargeCalculator.Summarize(property);
        var vm = new TransactionPageViewModel
        {
            ShowSystemApproved = true,
            PropertyId = property.Id,
            PropertyTitle = property.Title,
            PropertyLocation = property.Location,
            ListingType = property.ListingType,
            ListPrice = property.Price,
            ChargeSummaryLine = charges.SummaryLine,
            TotalDue = charges.TotalDue,
            RentAdvance = charges.RentAdvance,
            RentSecurityDeposit = charges.RentSecurityDeposit,
            BuyEarnestMoney = charges.BuyEarnestMoney,
            Form = new TransactionFormModel
            {
                PropertyId = property.Id,
                FullName = HttpContext.Session.GetString(TransactionSessionKeys.CustomerName) ?? string.Empty,
                Email = HttpContext.Session.GetString(TransactionSessionKeys.CustomerEmail) ?? string.Empty,
                Phone = HttpContext.Session.GetString(TransactionSessionKeys.CustomerPhone)
            }
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index([Bind(Prefix = "Form")] TransactionFormModel form)
    {
        var property = _catalog.GetProperty(form.PropertyId);
        if (property is null)
            return NotFound();

        var sessionPid = HttpContext.Session.GetString(TransactionSessionKeys.PropertyId);
        if (sessionPid != form.PropertyId.ToString())
        {
            TempData["TransactionError"] = "Your session expired or the transaction step was invalid. Please submit a viewing request again.";
            return RedirectToAction(nameof(PropertiesController.Details), "Properties", new { id = form.PropertyId });
        }

        if (!form.AcceptTerms)
            ModelState.AddModelError(nameof(form.AcceptTerms), "You must accept the terms to continue.");

        if (!ModelState.IsValid)
        {
            var chargesInvalid = TransactionChargeCalculator.Summarize(property);
            var vm = new TransactionPageViewModel
            {
                ShowSystemApproved = true,
                PropertyId = property.Id,
                PropertyTitle = property.Title,
                PropertyLocation = property.Location,
                ListingType = property.ListingType,
                ListPrice = property.Price,
                ChargeSummaryLine = chargesInvalid.SummaryLine,
                TotalDue = chargesInvalid.TotalDue,
                RentAdvance = chargesInvalid.RentAdvance,
                RentSecurityDeposit = chargesInvalid.RentSecurityDeposit,
                BuyEarnestMoney = chargesInvalid.BuyEarnestMoney,
                Form = form
            };
            return View(vm);
        }

        var reference = $"TRU-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        HttpContext.Session.Remove(TransactionSessionKeys.PropertyId);
        HttpContext.Session.Remove(TransactionSessionKeys.CustomerName);
        HttpContext.Session.Remove(TransactionSessionKeys.CustomerEmail);
        HttpContext.Session.Remove(TransactionSessionKeys.CustomerPhone);

        var charges = TransactionChargeCalculator.Summarize(property);
        TempData["TransactionReference"] = reference;
        TempData["TransactionPropertyTitle"] = property.Title;
        TempData["TransactionAmount"] = charges.TotalDue.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-PH"));
        return RedirectToAction(nameof(Success));
    }

    [HttpGet]
    public IActionResult Success()
    {
        var reference = TempData["TransactionReference"] as string;
        if (string.IsNullOrEmpty(reference))
            return RedirectToAction("Index", "Home");

        var vm = new TransactionSuccessViewModel
        {
            Reference = reference,
            PropertyTitle = TempData["TransactionPropertyTitle"] as string ?? "",
            AmountFormatted = TempData["TransactionAmount"] as string ?? ""
        };
        return View(vm);
    }
}
