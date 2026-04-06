using Microsoft.AspNetCore.Mvc;
using RealEstate.Models;
using RealEstate.Services;

namespace RealEstate.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    private readonly UserService _userService;

    public AdminController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("login")]
    public IActionResult Login(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Email and password are required");
            return View();
        }

        var user = _userService.LoginUser(email, password);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }

        // Set session
        HttpContext.Session.SetString("UserId", user.UserId.ToString());
        HttpContext.Session.SetString("UserEmail", user.Email);
        HttpContext.Session.SetString("UserName", user.FullName);
        HttpContext.Session.SetString("UserRole", user.Role?.RoleType ?? "Unknown");

        // Redirect based on role
        var userRole = user.Role?.RoleType;
        return userRole switch
        {
            "SuperAdmin" => RedirectToAction("Dashboard", "SuperAdmin"),
            "Manager" => RedirectToAction("Dashboard", "Manager"),
            "Accounting" => RedirectToAction("Dashboard", "Accounting"),
            "Investor" => RedirectToAction("Dashboard", "Investor"),
            "Agent" => Redirect("/agent"),
            "Broker" => RedirectToAction("Dashboard", "Broker"),
            _ => RedirectToAction("Dashboard", "Admin")
        };
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login");
        }

        return View();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        // Pass roles to view
        var roles = _userService.GetAllRoles();
        ViewBag.Roles = roles;
        return View();
    }

    [HttpPost("register")]
    public IActionResult Register(string fullName, string email, string password, string confirmPassword, string roleType)
    {
        // Validate input
        if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || 
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || 
            string.IsNullOrEmpty(roleType))
        {
            ModelState.AddModelError("", "All fields are required");
            ViewBag.Roles = _userService.GetAllRoles();
            return View();
        }

        if (password != confirmPassword)
        {
            ModelState.AddModelError("", "Passwords do not match");
            ViewBag.Roles = _userService.GetAllRoles();
            return View();
        }

        if (password.Length < 6)
        {
            ModelState.AddModelError("", "Password must be at least 6 characters");
            ViewBag.Roles = _userService.GetAllRoles();
            return View();
        }

        if (!email.Contains("@"))
        {
            ModelState.AddModelError("", "Invalid email format");
            ViewBag.Roles = _userService.GetAllRoles();
            return View();
        }

        try
        {
            // Register new user
            var newUser = _userService.RegisterUser(fullName, email, password, roleType);

            // Redirect back to register page with success flag to show modal
            return RedirectToAction("Register", new { registered = "true" });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.Roles = _userService.GetAllRoles();
            return View();
        }
    }
}

