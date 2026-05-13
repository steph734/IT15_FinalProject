using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RealEstate.Models;
using RealEstate.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace RealEstate.Controllers;

[Route("superadmin")]
public class SuperAdminController : Controller
{
    private readonly ApplicationDBContext _context;
    private readonly UserService _userService;
    private readonly ILogger<SuperAdminController> _logger;

    public SuperAdminController(ApplicationDBContext context, UserService userService, ILogger<SuperAdminController> logger)
    {
        _context = context;
        _userService = userService;
        _logger = logger;
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // LOGIN
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId").HasValue && HttpContext.Session.GetString("Role") == "SuperAdmin")
        {
            return RedirectToAction(nameof(Dashboard));
        }
        return View();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login(string email, string password)
    {
        try
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                return View();
            }

            if (!VerifyPassword(password, user.PasswordHash))
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                return View();
            }

            if (user.Role != "SuperAdmin")
            {
                TempData["ErrorMessage"] = "Access denied. SuperAdmin privileges required.";
                return View();
            }

            if (!user.IsActive)
            {
                TempData["ErrorMessage"] = "Your account has been deactivated. Please contact system administrator.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.Role);

            user.LastLogin = DateTime.UtcNow;
            _context.SaveChanges();

            _logger.LogInformation("SuperAdmin {Email} logged in successfully", email);
            TempData["SuccessMessage"] = $"Welcome back, {user.FullName}!";
            return RedirectToAction(nameof(Dashboard));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during SuperAdmin login");
            TempData["ErrorMessage"] = "An error occurred during login. Please try again.";
            return View();
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // DASHBOARD
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("dashboard")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult Dashboard()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        // Gather statistics
        var stats = new SuperAdminDashboardStats
        {
            TotalUsers = _context.Users.Count(),
            ActiveUsers = _context.Users.Count(u => u.IsActive),
            TotalProperties = _context.Properties.Count(),
            TotalListings = _context.Properties.Count(),
            PendingReviews = _context.Properties.Count(p => !p.IsApproved && !p.IsRejected),
            TotalSales = _context.Transactions.Count(),
            RecentUsers = _context.Users.OrderByDescending(u => u.CreatedAt).Take(5).ToList(),
            RecentActivity = _context.AuditLogs?.OrderByDescending(a => a.CreatedAt).Take(10).ToList() ?? new List<AuditLog>()
        };

        return View(stats);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // USER MANAGEMENT
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("users")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult Users(string? search, string? role, bool? isActive)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
        }

        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(u => u.Role == role);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        var users = query.OrderByDescending(u => u.CreatedAt).ToList();
        ViewBag.Roles = new[] { "Admin", "Manager", "Broker", "Seller", "Accounting", "SuperAdmin" };
        ViewBag.Search = search;
        ViewBag.Role = role;
        ViewBag.IsActive = isActive;

        return View(users);
    }

    [HttpGet("users/create")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult CreateUser()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        ViewBag.Roles = new[] { "Admin", "Manager", "Broker", "Seller", "Accounting" };
        return View();
    }

    [HttpPost("users/create")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult CreateUser(ApplicationUser user, string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
        {
            ModelState.AddModelError("Password", "Password must be at least 8 characters long.");
        }

        if (_context.Users.Any(u => u.Email == user.Email))
        {
            ModelState.AddModelError("Email", "Email already exists.");
        }

        if (!ModelState.IsValid)
        {
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");
            ViewBag.Roles = new[] { "Admin", "Manager", "Broker", "Seller", "Accounting" };
            return View(user);
        }

        user.PasswordHash = HashPassword(password);
        user.CreatedAt = DateTime.UtcNow;
        user.IsActive = true;

        _context.Users.Add(user);
        _context.SaveChanges();

        _logger.LogInformation("SuperAdmin created user {Email} with role {Role}", user.Email, user.Role);
        TempData["SuccessMessage"] = $"User '{user.FullName}' created successfully!";
        return RedirectToAction(nameof(Users));
    }

    [HttpGet("users/edit/{id:int}")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult EditUser(int id)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();

        ViewBag.Roles = new[] { "Admin", "Manager", "Broker", "Seller", "Accounting", "SuperAdmin" };
        return View(user);
    }

    [HttpPost("users/edit/{id:int}")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult EditUser(int id, ApplicationUser updatedUser, string? newPassword)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();

        if (!string.IsNullOrEmpty(newPassword))
        {
            if (newPassword.Length < 8)
            {
                ModelState.AddModelError("Password", "Password must be at least 8 characters long.");
            }
            else
            {
                user.PasswordHash = HashPassword(newPassword);
            }
        }

        if (!ModelState.IsValid)
        {
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");
            ViewBag.Roles = new[] { "Admin", "Manager", "Broker", "Seller", "Accounting", "SuperAdmin" };
            return View(user);
        }

        user.FullName = updatedUser.FullName;
        user.Email = updatedUser.Email;
        user.Role = updatedUser.Role;
        user.IsActive = updatedUser.IsActive;
        user.PhoneNumber = updatedUser.PhoneNumber;
        user.UpdatedAt = DateTime.UtcNow;

        _context.SaveChanges();

        _logger.LogInformation("SuperAdmin updated user {Email}", user.Email);
        TempData["SuccessMessage"] = $"User '{user.FullName}' updated successfully!";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost("users/delete/{id:int}")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();

        if (user.UserId == HttpContext.Session.GetInt32("UserId"))
        {
            TempData["ErrorMessage"] = "You cannot delete your own account!";
            return RedirectToAction(nameof(Users));
        }

        _context.Users.Remove(user);
        _context.SaveChanges();

        _logger.LogInformation("SuperAdmin deleted user {Email}", user.Email);
        TempData["SuccessMessage"] = $"User '{user.FullName}' deleted successfully!";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost("users/toggle-status/{id:int}")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult ToggleUserStatus(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();

        if (user.UserId == HttpContext.Session.GetInt32("UserId"))
        {
            TempData["ErrorMessage"] = "You cannot deactivate your own account!";
            return RedirectToAction(nameof(Users));
        }

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        _context.SaveChanges();

        var status = user.IsActive ? "activated" : "deactivated";
        _logger.LogInformation("SuperAdmin {status} user {Email}", status, user.Email);
        TempData["SuccessMessage"] = $"User '{user.FullName}' {status} successfully!";
        return RedirectToAction(nameof(Users));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // AUDIT LOGS
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("audit-logs")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult AuditLogs(DateTime? fromDate, DateTime? toDate, string? userEmail, string? action)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        var query = _context.AuditLogs?.AsQueryable() ?? Enumerable.Empty<AuditLog>().AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(l => l.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(l => l.CreatedAt <= toDate.Value.AddDays(1));

        if (!string.IsNullOrEmpty(userEmail))
            query = query.Where(l => l.User != null && l.User.Email.Contains(userEmail));

        if (!string.IsNullOrEmpty(action))
            query = query.Where(l => l.Action.Contains(action));

        var logs = query.OrderByDescending(l => l.CreatedAt).Take(500).ToList();
        return View(logs);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // SYSTEM SETTINGS
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("settings")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult Settings()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var settings = _context.SystemSettings?.FirstOrDefault() ?? new SystemSettings();
        return View(settings);
    }

    [HttpPost("settings")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult Settings(SystemSettings settings)
    {
        var existing = _context.SystemSettings?.FirstOrDefault();
        if (existing == null)
        {
            _context.SystemSettings?.Add(settings);
        }
        else
        {
            existing.SiteName = settings.SiteName;
            existing.SiteDescription = settings.SiteDescription;
            existing.ContactEmail = settings.ContactEmail;
            existing.SupportPhone = settings.SupportPhone;
            existing.DefaultCommissionRate = settings.DefaultCommissionRate;
            existing.MinimumPropertyPrice = settings.MinimumPropertyPrice;
            existing.EnableRegistration = settings.EnableRegistration;
            existing.MaintenanceMode = settings.MaintenanceMode;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        _context.SaveChanges();
        TempData["SuccessMessage"] = "System settings updated successfully!";
        return RedirectToAction(nameof(Settings));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // DATABASE MANAGEMENT
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("database")]
    [AuthorizeRole("SuperAdmin")]
    public IActionResult Database()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        return View();
    }

    // ══════════════════════════════════════════════════════════════════════════
    // LOGOUT
    // ══════════════════════════════════════════════════════════════════════════

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Redirect("https://estateflow.runasp.net/admin/login");
    }
}

// View Model for Dashboard
public class SuperAdminDashboardStats
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalProperties { get; set; }
    public int TotalListings { get; set; }
    public int PendingReviews { get; set; }
    public int TotalSales { get; set; }
    public List<ApplicationUser> RecentUsers { get; set; } = new();
    public List<AuditLog> RecentActivity { get; set; } = new();
}
