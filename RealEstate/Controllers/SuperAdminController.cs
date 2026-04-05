using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;

namespace RealEstate.Controllers;

[Route("superadmin")]
[AuthorizeRole("SuperAdmin")]
public class SuperAdminController : Controller
{
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Admin");
    }
}
