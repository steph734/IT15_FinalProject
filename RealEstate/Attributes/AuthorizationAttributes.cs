using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RealEstate.Attributes;

/// <summary>
/// Attribute to require user authentication for actions
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAuthenticationAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.Session.GetString("UserId");
        var userRole = context.HttpContext.Session.GetString("UserRole");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
        {
            context.Result = new RedirectToActionResult("Login", "Admin", new { });
        }
    }
}

/// <summary>
/// Attribute to require specific roles
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _allowedRoles;

    public RequireRoleAttribute(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.Session.GetString("UserId");
        var userRole = context.HttpContext.Session.GetString("UserRole");

        // Not authenticated
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
        {
            context.Result = new RedirectToActionResult("Login", "Admin", new { });
            return;
        }

        // Not authorized for this role
        if (!_allowedRoles.Contains(userRole))
        {
            context.Result = new ForbidResult();
        }
    }
}

/// <summary>
/// Attribute to require agent role
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAgentAttribute : RequireRoleAttribute
{
    public RequireAgentAttribute() : base("Agent") { }
}

/// <summary>
/// Attribute to require manager role
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireManagerAttribute : RequireRoleAttribute
{
    public RequireManagerAttribute() : base("Manager", "SuperAdmin") { }
}

/// <summary>
/// Attribute to require investor role
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireInvestorAttribute : RequireRoleAttribute
{
    public RequireInvestorAttribute() : base("Investor") { }
}

/// <summary>
/// Attribute to require admin role (SuperAdmin or Manager)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAdminAttribute : RequireRoleAttribute
{
    public RequireAdminAttribute() : base("SuperAdmin", "Manager") { }
}
