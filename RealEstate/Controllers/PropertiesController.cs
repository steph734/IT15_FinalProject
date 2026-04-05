using Microsoft.AspNetCore.Mvc;
using RealEstate.Models;
using RealEstate.Services;

namespace RealEstate.Controllers;

public class PropertiesController : Controller
{
    private readonly PropertyCatalog _catalog;
    private readonly SubscriptionService _subs;

    public PropertiesController(PropertyCatalog catalog)
    {
        _catalog = catalog;
    }

    public PropertiesController(PropertyCatalog catalog, SubscriptionService subs)
    {
        _catalog = catalog;
        _subs = subs;
    }

    public IActionResult Index(string? location, string? priceRange, decimal? maxPrice, int page = 1, int pageSize = 5)
    {
        priceRange = string.IsNullOrWhiteSpace(priceRange) ? "any" : priceRange;

        var filtered = _catalog.Filter(location, priceRange, maxPrice);
        var total = filtered.Count;

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var paged = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var vm = new PropertiesIndexViewModel
        {
            Location = location,
            PriceRange = priceRange,
            MaxPrice = maxPrice,
            Properties = paged,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
        return View(vm);
    }

    public IActionResult Details(int id)
    {
        var property = _catalog.GetProperty(id);
        if (property is null)
            return NotFound();

        var vm = new PropertyDetailsViewModel
        {
            Property = property,
            Agent = _catalog.GetAgent(property.AgentId),
            Schedule = new ScheduleViewingViewModel { PropertyId = property.Id }
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ScheduleViewing([Bind(Prefix = "Schedule")] ScheduleViewingViewModel model)
    {
        var property = _catalog.GetProperty(model.PropertyId);
        if (property is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            return View("Details", new PropertyDetailsViewModel
            {
                Property = property,
                Agent = _catalog.GetAgent(property.AgentId),
                Schedule = model
            });
        }

        HttpContext.Session.SetString(TransactionSessionKeys.PropertyId, model.PropertyId.ToString());
        HttpContext.Session.SetString(TransactionSessionKeys.CustomerName, model.Name);
        HttpContext.Session.SetString(TransactionSessionKeys.CustomerEmail, model.Email);
        HttpContext.Session.SetString(TransactionSessionKeys.CustomerPhone, model.Phone ?? string.Empty);

        return RedirectToAction("Index", "Transaction", new { propertyId = model.PropertyId });
    }
}
