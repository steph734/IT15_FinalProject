using Microsoft.AspNetCore.Mvc;

namespace RealEstate.Helpers;

/// <summary>
/// Helper class for role-based authorization checks
/// </summary>
public static class AuthorizationHelper
{
    /// <summary>
    /// Check if user is authenticated and has a valid role
    /// </summary>
    public static bool IsUserAuthenticated(HttpContext httpContext)
    {
        var userId = httpContext.Session.GetString("UserId");
        var userRole = httpContext.Session.GetString("UserRole");
        return !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userRole);
    }

    /// <summary>
    /// Get current user's role
    /// </summary>
    public static string? GetUserRole(HttpContext httpContext)
    {
        return httpContext.Session.GetString("UserRole");
    }

    /// <summary>
    /// Get current user ID
    /// </summary>
    public static int? GetUserId(HttpContext httpContext)
    {
        var userIdStr = httpContext.Session.GetString("UserId");
        return int.TryParse(userIdStr, out var userId) ? userId : null;
    }

    /// <summary>
    /// Check if user has any of the specified roles
    /// </summary>
    public static bool HasRole(HttpContext httpContext, params string[] roles)
    {
        var userRole = GetUserRole(httpContext);
        return userRole != null && roles.Contains(userRole);
    }

    /// <summary>
    /// Check if user is an agent
    /// </summary>
    public static bool IsAgent(HttpContext httpContext)
    {
        return HasRole(httpContext, "Agent");
    }

    /// <summary>
    /// Check if user is a manager
    /// </summary>
    public static bool IsManager(HttpContext httpContext)
    {
        return HasRole(httpContext, "Manager");
    }

    /// <summary>
    /// Check if user is an investor
    /// </summary>
    public static bool IsInvestor(HttpContext httpContext)
    {
        return HasRole(httpContext, "Investor");
    }

    /// <summary>
    /// Check if user is an admin (SuperAdmin or Manager)
    /// </summary>
    public static bool IsAdmin(HttpContext httpContext)
    {
        return HasRole(httpContext, "SuperAdmin", "Manager");
    }
}
