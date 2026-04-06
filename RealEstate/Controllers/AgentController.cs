using Microsoft.AspNetCore.Mvc;
using RealEstate.Attributes;
using RealEstate.Helpers;
using RealEstate.Services;

namespace RealEstate.Controllers;

[Route("agent")]
[RequireAgent]
public class AgentController : Controller
{
    private readonly PropertyCatalog _catalog;
    private readonly InquiryService _inquiryService;

    public AgentController(PropertyCatalog catalog, InquiryService inquiryService)
    {
        _catalog = catalog;
        _inquiryService = inquiryService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToAction("Dashboard");
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var agentId = AuthorizationHelper.GetUserId(HttpContext);
        if (!agentId.HasValue)
            return RedirectToAction("Login", "Admin");

        var agent = _catalog.GetAgent(agentId.Value);
        if (agent == null)
            return Unauthorized();

        ViewBag.AgentId = agentId.Value;
        ViewBag.AgentName = agent.Name;
        return View();
    }

    [HttpGet("myproperties")]
    public IActionResult MyProperties(int page = 1, int pageSize = 10)
    {
        var agentId = AuthorizationHelper.GetUserId(HttpContext);
        if (!agentId.HasValue)
            return RedirectToAction("Login", "Admin");

        var agent = _catalog.GetAgent(agentId.Value);
        if (agent == null)
            return Unauthorized();

        // Get properties for this agent
        var allProperties = _catalog.GetProperties()
            .Where(p => p.AgentId == agentId.Value)
            .ToList();

        var total = allProperties.Count;
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var paged = allProperties
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.AgentId = agentId.Value;
        ViewBag.AgentName = agent.Name;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = total;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

        return View(paged);
    }

    [HttpGet("inquiries")]
    public IActionResult Inquiries()
    {
        var agentId = AuthorizationHelper.GetUserId(HttpContext);
        if (!agentId.HasValue)
            return RedirectToAction("Login", "Admin");

        var agent = _catalog.GetAgent(agentId.Value);
        if (agent == null)
            return Unauthorized();

        ViewBag.AgentId = agentId.Value;
        ViewBag.AgentName = agent.Name;
        return View();
    }

    [HttpGet("clients")]
    public IActionResult Clients()
    {
        var agentId = AuthorizationHelper.GetUserId(HttpContext);
        if (!agentId.HasValue)
            return RedirectToAction("Login", "Admin");

        var agent = _catalog.GetAgent(agentId.Value);
        if (agent == null)
            return Unauthorized();

        ViewBag.AgentId = agentId.Value;
        ViewBag.AgentName = agent.Name;
        return View();
    }

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var agentId = AuthorizationHelper.GetUserId(HttpContext);
        if (!agentId.HasValue)
            return RedirectToAction("Login", "Admin");

        var agent = _catalog.GetAgent(agentId.Value);
        if (agent == null)
            return Unauthorized();

        return View(agent);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Admin");
    }
}
