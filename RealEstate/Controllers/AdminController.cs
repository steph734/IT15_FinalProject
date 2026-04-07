using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Models;
using RealEstate.Services;
using Microsoft.Extensions.Configuration;

namespace RealEstate.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;

    public AdminController(UserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Email and password are required");
            return View();
        }

        var recaptchaResponse = Request.Form["g-recaptcha-response"];
        var secretKey = _configuration["RecaptchaSettings:SecretKey"];
        using var client = new HttpClient();
        var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}", null);
        var jsonString = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(jsonString);
        if (!jsonResponse.GetProperty("success").GetBoolean())
        {
            ModelState.AddModelError("", "Please verify that you are not a robot.");
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
    public async Task<IActionResult> Register(string fullName, string email, string password, string confirmPassword, string roleType)
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

        var recaptchaResponse = Request.Form["g-recaptcha-response"];
        var secretKey = _configuration["RecaptchaSettings:SecretKey"];
        using var client = new HttpClient();
        var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}", null);
        var jsonString = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(jsonString);
        if (!jsonResponse.GetProperty("success").GetBoolean())
        {
            ModelState.AddModelError("", "Please verify that you are not a robot.");
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

        // Validate email domain
        var validDomains = new[] { "gmail.com", "yahoo.com", "outlook.com", "hotmail.com" };
        var emailDomain = email.Split("@")[1].ToLower();
        if (!validDomains.Contains(emailDomain))
        {
            ModelState.AddModelError("", $"Email domain must be one of: {string.Join(", ", validDomains)}. Please use a valid email provider.");
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

