using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;

namespace RealEstate.Controllers;

[Route("manager")]
[AuthorizeRole("Manager")]
public class ManagerController : Controller
{
    public ManagerController()
    {
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Manager/Dashboard.cshtml");
    }

    [HttpGet("team-management")]
    public IActionResult TeamManagement()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Manager/TeamManagement.cshtml");
    }

    [HttpGet("agent-assignment")]
    public IActionResult AgentAssignment()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Manager/AgentAssignment.cshtml");
    }

    [HttpGet("property-moderation")]
    public IActionResult PropertyModeration()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Manager/PropertyModeration.cshtml");
    }

    [HttpGet("sales-pipeline")]
    public IActionResult SalesPipeline()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Manager/SalesPipeline.cshtml");
    }

    [HttpGet("inventory-overview")]
    public IActionResult InventoryOverview()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Manager/InventoryOverview.cshtml");
    }

    [HttpGet("performance-tracking")]
    public IActionResult PerformanceTracking()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Manager/PerformanceTracking.cshtml");
    }

    [HttpGet("managerial-reports")]
    public IActionResult ManagerialReports()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Manager/ManagerialReports.cshtml");
    }

    [HttpGet("settings")]
    public IActionResult Settings()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Manager/Settings.cshtml");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Admin");
    }
}
