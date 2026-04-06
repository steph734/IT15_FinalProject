using Microsoft.AspNetCore.Mvc;
using RealEstate.Attributes;
using RealEstate.Helpers;
using RealEstate.Services;

namespace RealEstate.Controllers;

[Route("broker")]
[RequireRole("Broker")]
public class BrokerController : Controller
{
    private readonly PropertyCatalog _catalog;

    public BrokerController(PropertyCatalog catalog)
    {
        _catalog = catalog;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToAction("Dashboard");
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("properties")]
    public IActionResult Properties(int page = 1, int pageSize = 10)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        var allProperties = _catalog.GetProperties().ToList();
        var total = allProperties.Count;
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var paged = allProperties
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = total;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

        return View(paged);
    }

    [HttpGet("leads")]
    public IActionResult Leads()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("performance")]
    public IActionResult Performance()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("commissions")]
    public IActionResult Commissions()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("settings")]
    public IActionResult Settings()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Admin");
    }
}
