using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RealEstate.Services;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _allowedRoles;

    public AuthorizeRoleAttribute(params string[] roles)
    {
        _allowedRoles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userRole = context.HttpContext.Session.GetString("UserRole");

        // If not logged in
        if (string.IsNullOrEmpty(userRole))
        {
            context.Result = new RedirectToActionResult("Login", "Admin", null);
            return;
        }

        // If role is not in allowed roles
        if (!_allowedRoles.Contains(userRole))
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
