using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;

namespace RealEstate.Controllers;

[Route("subscriptions")]
public class SubscriptionsController : Controller
{
    private readonly SubscriptionService _subs;

    public SubscriptionsController(SubscriptionService subs)
    {
        _subs = subs;
    }

    [HttpPost("set")]
    [ValidateAntiForgeryToken]
    public IActionResult Set(string tier)
    {
        // For demo: use session id as key
        var key = HttpContext.Session.Id ?? Guid.NewGuid().ToString();
        if (string.Equals(tier, "Plus", StringComparison.OrdinalIgnoreCase))
            _subs.SetSubscription(key, SubscriptionTier.Plus);
        else if (string.Equals(tier, "Premium", StringComparison.OrdinalIgnoreCase))
            _subs.SetSubscription(key, SubscriptionTier.Premium);
        else
            _subs.SetSubscription(key, SubscriptionTier.Free);

        TempData["SubscriptionSet"] = "Subscription updated.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var key = HttpContext.Session.Id ?? Guid.NewGuid().ToString();
        var tier = _subs.GetSubscription(key);
        var vm = new RealEstate.Models.SubscriptionViewModel { CurrentTier = tier };
        return View("~/Views/Subscriptions/Index.cshtml", vm);
    }
}
