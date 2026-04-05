using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;

namespace RealEstate.Controllers;

[Route("broker")]
[AuthorizeRole("Broker")]
public class BrokerController : Controller
{
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Broker/Dashboard.cshtml");
    }

    [HttpGet("listings")]
    public IActionResult Listings()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Broker/Listings.cshtml");
    }

    [HttpGet("clients")]
    public IActionResult Clients()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Broker/Clients.cshtml");
    }

    [HttpGet("sales")]
    public IActionResult Sales()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Broker/Sales.cshtml");
    }

    [HttpGet("commissions")]
    public IActionResult Commissions()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Broker/Commissions.cshtml");
    }

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Broker/Profile.cshtml");
    }

    [HttpGet("settings")]
    public IActionResult Settings()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Broker/Settings.cshtml");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Admin");
    }
}
