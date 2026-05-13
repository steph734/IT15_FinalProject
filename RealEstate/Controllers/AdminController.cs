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
    public async Task<IActionResult> Login(string email, string password, string gRecaptchaResponse)
    {
        // Always return JSON for AJAX/fetch requests
        bool isJson = IsJsonRequest();

        // Also check for g-recaptcha-response form field name (what Google reCAPTCHA uses)
        if (string.IsNullOrEmpty(gRecaptchaResponse))
        {
            gRecaptchaResponse = Request.Form["g-recaptcha-response"].ToString();
        }

        // Verify reCAPTCHA
        if (string.IsNullOrEmpty(gRecaptchaResponse))
        {
            if (isJson)
                return Json(new { success = false, message = "Please complete the reCAPTCHA verification" });

            ModelState.AddModelError("", "Please check the 'I'm not a robot' checkbox");
            return View();
        }

        // Verify reCAPTCHA with Google
        var recaptchaValid = await VerifyRecaptcha(gRecaptchaResponse);
        if (!recaptchaValid)
        {
            if (isJson)
                return Json(new { success = false, message = "reCAPTCHA verification failed. Please try again." });

            ModelState.AddModelError("", "reCAPTCHA verification failed. Please try again.");
            return View();
        }

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (isJson)
                return Json(new { success = false, message = "Username and password are required" });

            ModelState.AddModelError("", "Username and password are required");
            return View();
        }

        try
        {
            // First check if user exists with these credentials
            var userCheck = _userService.GetUserByEmail(email) ?? _userService.GetUserByUsername(email);
            
            if (userCheck != null)
            {
                // User exists - check if password is correct
                var passwordValid = _userService.VerifyPasswordForUser(userCheck.UserId, password);
                if (passwordValid && !userCheck.IsActive)
                {
                    TempData["ErrorMessage"] = "Your account is not activated. Please verify your email first.";
                    if (isJson)
                        return Json(new { success = false, message = "Account not activated. Please verify your email." });
                    ModelState.AddModelError("", "Your account is not activated. Please verify your email first.");
                    return View();
                }
            }

            var user = _userService.LoginUser(email, password);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid username or password. Please try again.";
                
                if (isJson)
                    return Json(new { success = false, message = "Invalid username or password" });

                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            // Store user data in session
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role ?? "Unknown");

            var role = user.Role ?? "Unknown";

            // Set success message for toast notification
            TempData["SuccessMessage"] = $"Welcome back, {user.FullName}!";

            if (isJson)
                return Json(new {
                    success = true,
                    message = "Login successful",
                    userId = user.UserId,
                    email = user.Email,
                    fullName = user.FullName,
                    role = role
                });

            // Redirect to appropriate dashboard based on role
            return role switch
            {
                "SuperAdmin"  => RedirectToAction("Dashboard", "SuperAdmin"),
                "Broker"      => RedirectToAction("MyDashboard", "Broker"),
                "Agent"       => RedirectToAction("MyDashboard", "Broker"),
                "Manager"     => RedirectToAction("Dashboard", "Manager"),
                "Accounting"  => RedirectToAction("Dashboard", "Accounting"),
                "Investor"    => RedirectToAction("Dashboard", "Investor"),
                "Seller"      => RedirectToAction("Dashboard", "Seller"),
                _             => RedirectToAction("Index", "Home")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for user: {Email}", email);
            TempData["ErrorMessage"] = "A server error occurred. Please try again later.";

            if (isJson)
                return Json(new { success = false, message = "A server error occurred. Please try again." });

            ModelState.AddModelError("", "A server error occurred. Please try again.");
            return View();
        }
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

        // OTP is valid — activate the account now that email is confirmed
        _userService.ActivateUser(userId);

        var tempUserRole  = HttpContext.Session.GetString("TempUserRole");
        var tempUserName  = HttpContext.Session.GetString("TempUserName");
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
            "Agent" => "/broker/dashboard",
            "Broker" => "/broker/dashboard",
            "Seller" => "/seller/dashboard",
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
            "Agent" => RedirectToAction("Dashboard", "Broker"),
            "Broker" => RedirectToAction("Dashboard", "Broker"),
            "Seller" => RedirectToAction("Dashboard", "Seller"),
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
    public async Task<IActionResult> Register(string fullName, string username, string email, string password, string confirmPassword, string role)
    {
        bool isJson = IsJsonRequest();

        // ── Field validation ──────────────────────────────────────────────
        if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(role))
        {
            if (isJson) return Json(new { success = false, message = "All fields are required." });
            ModelState.AddModelError("", "All fields are required");
            ViewBag.Roles = _userService.GetAllRoles();
            return View();
        }

        // ── Role validation ───────────────────────────────────────────────
        var allowedRoles = _userService.GetAllRoles();
        if (!allowedRoles.Contains(role))
        {
            if (isJson) return Json(new { success = false, message = "Please select a valid role." });
            ModelState.AddModelError("", "Please select a valid role");
            ViewBag.Roles = allowedRoles;
            return View();
        }

        // ── Password checks ───────────────────────────────────────────────
        if (password != confirmPassword)
        {
            if (isJson) return Json(new { success = false, message = "Passwords do not match." });
            ModelState.AddModelError("", "Passwords do not match");
            ViewBag.Roles = allowedRoles;
            return View();
        }

        if (password.Length < 6)
        {
            if (isJson) return Json(new { success = false, message = "Password must be at least 6 characters." });
            ModelState.AddModelError("", "Password must be at least 6 characters");
            ViewBag.Roles = allowedRoles;
            return View();
        }

        if (username.Length < 3)
        {
            if (isJson) return Json(new { success = false, message = "Username must be at least 3 characters." });
            ModelState.AddModelError("", "Username must be at least 3 characters");
            ViewBag.Roles = allowedRoles;
            return View();
        }

        if (!email.Contains("@"))
        {
            if (isJson) return Json(new { success = false, message = "Invalid email format." });
            ModelState.AddModelError("", "Invalid email format");
            ViewBag.Roles = allowedRoles;
            return View();
        }

        // ── Email domain validation ───────────────────────────────────────
        var validDomains = new[] { "gmail.com", "yahoo.com", "outlook.com", "hotmail.com" };
        var emailDomain = email.Split("@")[1].ToLower();
        if (!validDomains.Contains(emailDomain))
        {
            var domainMsg = $"Email domain must be one of: {string.Join(", ", validDomains)}.";
            if (isJson) return Json(new { success = false, message = domainMsg });
            ModelState.AddModelError("", domainMsg);
            ViewBag.Roles = allowedRoles;
            return View();
        }

        // ── reCAPTCHA verification ────────────────────────────────────────
        try
        {
            var recaptchaResponse = Request.Form["g-recaptcha-response"].ToString();
            var secretKey = _configuration["RecaptchaSettings:SecretKey"];
            using var client = new HttpClient();
            var rcResponse = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}", null);
            var jsonString = await rcResponse.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(jsonString);
            if (!jsonResponse.GetProperty("success").GetBoolean())
            {
                if (isJson) return Json(new { success = false, message = "Please verify that you are not a robot." });
                ModelState.AddModelError("", "Please verify that you are not a robot.");
                ViewBag.Roles = allowedRoles;
                return View();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "reCAPTCHA verification failed");
            if (isJson) return Json(new { success = false, message = "reCAPTCHA verification failed. Please try again." });
            ModelState.AddModelError("", "reCAPTCHA verification failed. Please try again.");
            ViewBag.Roles = allowedRoles;
            return View();
        }

        // ── Register user & send OTP ──────────────────────────────────────
        try
        {
            var newUser = _userService.RegisterUser(fullName, username, email, password, role);

            var otpCode  = await _otpService.GenerateOtpAsync(newUser.UserId);
            var emailSent = await _emailService.SendOtpEmailAsync(newUser.Email, otpCode, newUser.FullName);

            if (!emailSent)
            {
                _logger.LogWarning("OTP email failed for {Email} — activating account directly (email service unavailable)", newUser.Email);

                // Email service unavailable — activate the account immediately so the user can log in
                _userService.ActivateUser(newUser.UserId);

                if (isJson)
                    return Json(new {
                        success = true,
                        skipOtp = true,
                        userId = newUser.UserId,
                        email = newUser.Email,
                        message = "Account created! Email verification is temporarily unavailable, but your account is active. You can log in now."
                    });

                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction("Login");
            }

            HttpContext.Session.SetString("TempUserId",    newUser.UserId.ToString());
            HttpContext.Session.SetString("TempUserEmail", newUser.Email);
            HttpContext.Session.SetString("TempUserName",  newUser.FullName);
            HttpContext.Session.SetString("TempUserRole",  newUser.Role ?? "Unknown");
            HttpContext.Session.SetString("OtpSentTime",   DateTime.UtcNow.ToString("o"));

            var expirySeconds = await _otpService.GetOtpExpirySecondsAsync(newUser.UserId);

            if (isJson)
                return Json(new { success = true, userId = newUser.UserId, email = newUser.Email, expirySeconds = expirySeconds });

            TempData["SuccessMessage"] = "Registration successful! Please verify your email.";
            return RedirectToAction("VerifyOtp");
        }
        catch (InvalidOperationException ex)
        {
            // e.g. "Username/email already exists"
            if (isJson) return Json(new { success = false, message = ex.Message });
            ModelState.AddModelError("", ex.Message);
            ViewBag.Roles = allowedRoles;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error for {Email}", email);
            if (isJson) return Json(new { success = false, message = "A server error occurred during registration. Please try again." });
            ModelState.AddModelError("", "A server error occurred. Please try again.");
            ViewBag.Roles = allowedRoles;
            return View();
        }
    }

    private async Task<bool> VerifyRecaptcha(string gRecaptchaResponse)
    {
        try
        {
            var secretKey = _configuration["RecaptchaSettings:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                _logger.LogWarning("reCAPTCHA SecretKey not configured");
                return true; // Allow if not configured (for development)
            }

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={gRecaptchaResponse}",
                null
            );

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;

            return root.GetProperty("success").GetBoolean();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying reCAPTCHA");
            return false;
        }
    }
}

