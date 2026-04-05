using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;

namespace RealEstate.Controllers;

[Route("accounting")]
[AuthorizeRole("Accounting")]
public class AccountingController : Controller
{
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Accounting/Dashboard.cshtml");
    }

    [HttpGet("transactions")]
    public IActionResult Transactions()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Accounting/Transactions.cshtml");
    }

    [HttpGet("invoices")]
    public IActionResult Invoices()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Accounting/Invoices.cshtml");
    }

    [HttpGet("reports")]
    public IActionResult Reports()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Accounting/Reports.cshtml");
    }

    [HttpGet("reconciliation")]
    public IActionResult Reconciliation()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Accounting/Reconciliation.cshtml");
    }

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Accounting/Profile.cshtml");
    }

    [HttpGet("settings")]
    public IActionResult Settings()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Accounting/Settings.cshtml");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Admin");
    }
}
