using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;

namespace RealEstate.Controllers;

public class AgentsController : Controller
{
    private readonly PropertyCatalog _catalog;

    public AgentsController(PropertyCatalog catalog)
    {
        _catalog = catalog;
    }

    public IActionResult Index()
    {
        return View(_catalog.GetAgents());
    }
}
