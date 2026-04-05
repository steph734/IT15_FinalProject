using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;
using RealEstate.Models;
using System.Text.Json;

namespace RealEstate.Controllers;

[Route("favorites")]
public class FavoritesController : Controller
{
    private const string SessionKey = "Wishlist";
    private readonly PropertyCatalog _catalog;

    public FavoritesController(PropertyCatalog catalog)
    {
        _catalog = catalog;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var ids = GetWishlistIds();
        var props = ids.Select(id => _catalog.GetProperty(id)).Where(p => p is not null).Cast<Property>().ToList();
        return View("~/Views/Favorites/Index.cshtml", props);
    }

    [HttpPost("add")]
    public IActionResult Add(int id)
    {
        var ids = GetWishlistIds();
        if (!ids.Contains(id)) ids.Add(id);
        SaveWishlistIds(ids);
        return Ok(new { count = ids.Count });
    }

    [HttpPost("remove")]
    public IActionResult Remove(int id)
    {
        var ids = GetWishlistIds();
        if (ids.Contains(id)) ids.Remove(id);
        SaveWishlistIds(ids);
        return Ok(new { count = ids.Count });
    }

    private List<int> GetWishlistIds()
    {
        var s = HttpContext.Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(s)) return new List<int>();
        try
        {
            return JsonSerializer.Deserialize<List<int>>(s) ?? new List<int>();
        }
        catch
        {
            return new List<int>();
        }
    }

    private void SaveWishlistIds(List<int> ids)
    {
        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(ids));
    }
}
