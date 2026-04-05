using Microsoft.AspNetCore.Mvc;
using RealEstate.Models.Investor;
using RealEstate.Services;

namespace RealEstate.Controllers;

[Route("investor")]
[AuthorizeRole("Investor")]
public class InvestorController : Controller
{
    private readonly UserService _userService;

    public InvestorController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var userName = HttpContext.Session.GetString("UserName");

        ViewData["UserName"] = userName;
        ViewData["UserId"] = userId;

        // Create the proper view model with sample data
        var viewModel = new InvestorDashboardViewModel
        {
            TotalPortfolioValue = 0,
            MonthlyIncome = 0,
            PropertyCount = 0
        };

        return View("~/Views/Investor/Dashboard.cshtml", viewModel);
    }

    [HttpGet("my-properties")]
    public IActionResult MyProperties()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Investor/MyProperties.cshtml");
    }

    [HttpGet("billing")]
    public IActionResult Billing()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Investor/Billing.cshtml");
    }

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Investor/Profile.cshtml");
    }

    [HttpGet("revision-center")]
    public IActionResult RevisionCenter()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Investor/RevisionCenter.cshtml");
    }

    [HttpGet("submit")]
    public IActionResult Submit()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Investor/Submit.cshtml");
    }

    [HttpGet("support")]
    public IActionResult Support()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Investor/Support.cshtml");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Admin");
    }
}
