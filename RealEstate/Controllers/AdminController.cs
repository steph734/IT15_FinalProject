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
    private readonly OtpService _otpService;
    private readonly EmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminController> _logger;

    public AdminController(UserService userService, OtpService otpService, EmailService emailService, IConfiguration configuration, ILogger<AdminController> logger)
    {
        _userService = userService;
        _otpService = otpService;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
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
            if (IsJsonRequest())
                return Json(new { success = false, message = "Email and password are required" });

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
            if (IsJsonRequest())
                return Json(new { success = false, message = "Please verify that you are not a robot." });

            ModelState.AddModelError("", "Please verify that you are not a robot.");
            return View();
        }

        var user = _userService.LoginUser(email, password);

        if (user == null)
        {
            if (IsJsonRequest())
                return Json(new { success = false, message = "Invalid email or password" });

            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }

        // Generate OTP
        var otpCode = await _otpService.GenerateOtpAsync(user.UserId);

        // Send OTP via email
        var emailSent = await _emailService.SendOtpEmailAsync(user.Email, otpCode, user.FullName);

        if (!emailSent)
        {
            _logger.LogWarning($"Failed to send OTP email to {user.Email}");
            if (IsJsonRequest())
                return Json(new { success = false, message = "Failed to send OTP email. Please try again." });

            ModelState.AddModelError("", "Failed to send OTP email. Please try again.");
            return View();
        }

        // Store temporary session data for OTP verification
        HttpContext.Session.SetString("TempUserId", user.UserId.ToString());
        HttpContext.Session.SetString("TempUserEmail", user.Email);
        HttpContext.Session.SetString("TempUserName", user.FullName);
        HttpContext.Session.SetString("TempUserRole", user.Role?.RoleType ?? "Unknown");
        HttpContext.Session.SetString("OtpSentTime", DateTime.UtcNow.ToString("o"));

        var expirySeconds = await _otpService.GetOtpExpirySecondsAsync(user.UserId);

        if (IsJsonRequest())
            return Json(new { success = true, userId = user.UserId, email = user.Email, expirySeconds = expirySeconds });

        // Redirect to OTP verification page for non-AJAX requests
        return RedirectToAction("VerifyOtp");
    }

    private bool IsJsonRequest()
    {
        return Request.Headers["Accept"].ToString().Contains("application/json") || 
               Request.ContentType?.Contains("application/json") == true;
    }

    [HttpGet("verify-otp")]
    public IActionResult VerifyOtp()
    {
        var tempUserId = HttpContext.Session.GetString("TempUserId");
        if (string.IsNullOrEmpty(tempUserId))
        {
            return RedirectToAction("Login");
        }

        var model = new OtpVerificationViewModel
        {
            Email = HttpContext.Session.GetString("TempUserEmail") ?? "",
            UserId = int.Parse(tempUserId)
        };

        return View(model);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(int userId, string otpCode)
    {
        if (string.IsNullOrEmpty(otpCode))
        {
            if (IsJsonRequest())
                return Json(new { success = false, message = "OTP code is required" });

            ModelState.AddModelError("", "OTP code is required");
            return View(new OtpVerificationViewModel { UserId = userId, Email = HttpContext.Session.GetString("TempUserEmail") ?? "" });
        }

        // Verify OTP
        var isValidOtp = await _otpService.VerifyOtpAsync(userId, otpCode);

        if (!isValidOtp)
        {
            if (IsJsonRequest())
                return Json(new { success = false, message = "Invalid or expired OTP. Please try again." });

            ModelState.AddModelError("", "Invalid or expired OTP. Please try again.");
            return View(new OtpVerificationViewModel { UserId = userId, Email = HttpContext.Session.GetString("TempUserEmail") ?? "" });
        }

        // OTP is valid, complete the login
        var tempUserRole = HttpContext.Session.GetString("TempUserRole");
        var tempUserName = HttpContext.Session.GetString("TempUserName");
        var tempUserEmail = HttpContext.Session.GetString("TempUserEmail");

        // Set permanent session
        HttpContext.Session.SetString("UserId", userId.ToString());
        HttpContext.Session.SetString("UserEmail", tempUserEmail);
        HttpContext.Session.SetString("UserName", tempUserName);
        HttpContext.Session.SetString("UserRole", tempUserRole);

        // Clear temporary session data
        HttpContext.Session.Remove("TempUserId");
        HttpContext.Session.Remove("TempUserEmail");
        HttpContext.Session.Remove("TempUserName");
        HttpContext.Session.Remove("TempUserRole");
        HttpContext.Session.Remove("OtpSentTime");

        // Determine redirect URL based on role
        string redirectUrl = tempUserRole switch
        {
            "SuperAdmin" => "/superadmin/dashboard",
            "Manager" => "/manager/dashboard",
            "Accounting" => "/accounting/dashboard",
            "Investor" => "/investor/dashboard",
            "Agent" => "/agent",
            "Broker" => "/broker/dashboard",
            _ => "/admin/dashboard"
        };

        if (IsJsonRequest())
            return Json(new { success = true, redirectUrl = redirectUrl });

        return tempUserRole switch
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

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp(int userId)
    {
        var tempUserId = HttpContext.Session.GetString("TempUserId");
        if (string.IsNullOrEmpty(tempUserId) || int.Parse(tempUserId) != userId)
        {
            return Json(new { success = false, message = "Invalid session. Please login again." });
        }

        var tempUserEmail = HttpContext.Session.GetString("TempUserEmail");
        var tempUserName = HttpContext.Session.GetString("TempUserName");

        if (string.IsNullOrEmpty(tempUserEmail) || string.IsNullOrEmpty(tempUserName))
        {
            return Json(new { success = false, message = "Invalid session data." });
        }

        try
        {
            // Generate new OTP
            var otpCode = await _otpService.GenerateOtpAsync(userId);

            // Send OTP via email
            var emailSent = await _emailService.SendOtpEmailAsync(tempUserEmail, otpCode, tempUserName);

            if (!emailSent)
            {
                _logger.LogWarning($"Failed to resend OTP email to {tempUserEmail}");
                return Json(new { success = false, message = "Failed to send OTP email. Please try again." });
            }

            var expirySeconds = await _otpService.GetOtpExpirySecondsAsync(userId);
            HttpContext.Session.SetString("OtpSentTime", DateTime.UtcNow.ToString("o"));

            return Json(new { success = true, message = "OTP sent successfully", expirySeconds = expirySeconds });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error resending OTP: {ex.Message}");
            return Json(new { success = false, message = "Failed to resend OTP. Please try again." });
        }
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

