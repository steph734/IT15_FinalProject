using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RealEstate.Models.Admin;

namespace RealEstate.Controllers.Admin;

[Route("admin")]
public class AdminController : AdminBaseController
{
    private readonly AdminAuthOptions _auth;

    public AdminController(IOptions<AdminAuthOptions> authOptions)
    {
        _auth = authOptions.Value;
    }

    [HttpGet]
    [Route("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (IsBrokerPortalSignedIn())
        {
            if (IsLocalReturnUrl(returnUrl))
                return Redirect(returnUrl!);
            return RedirectToAction(nameof(Dashboard));
        }

        return View(new AdminLoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [Route("login")]
    [ValidateAntiForgeryToken]
    public IActionResult Login(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!_auth.TryValidate(model.Username, model.Password, out var normalizedUser))
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        HttpContext.Session.SetString(AdminSessionKeys.SignedIn, "1");
        HttpContext.Session.SetString(AdminSessionKeys.SignedInAtUtc, DateTime.UtcNow.ToString("O"));
        HttpContext.Session.SetString(AdminSessionKeys.Username, normalizedUser ?? model.Username.Trim());

        if (IsLocalReturnUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl!);

        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [Route("logout")]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove(AdminSessionKeys.SignedIn);
        HttpContext.Session.Remove(AdminSessionKeys.SignedInAtUtc);
        HttpContext.Session.Remove(AdminSessionKeys.Username);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [Route("")]
    [Route("dashboard")]
    public IActionResult Dashboard()
    {
        if (!IsBrokerPortalSignedIn())
            return RedirectToAction(nameof(Login), new { returnUrl = Url.Action(nameof(Dashboard)) });

        ViewData["BrokerUsername"] = SignedInUsername();
        return View();
    }

    private bool IsLocalReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            return false;
        if (!Url.IsLocalUrl(returnUrl))
            return false;
        return !returnUrl.Contains("//", StringComparison.Ordinal);
    }
}
