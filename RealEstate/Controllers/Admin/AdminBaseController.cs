using Microsoft.AspNetCore.Mvc;
using RealEstate.Models.Admin;

namespace RealEstate.Controllers.Admin;

public abstract class AdminBaseController : Controller
{
    protected bool IsBrokerPortalSignedIn() =>
        HttpContext.Session.GetString(AdminSessionKeys.SignedIn) == "1";

    protected string? SignedInUsername() => HttpContext.Session.GetString(AdminSessionKeys.Username);

    protected IActionResult BrokerPortalLoginRedirect()
    {
        var returnUrl = Request.Path + Request.QueryString;
        return RedirectToAction("Login", "Admin", new { returnUrl });
    }
}
