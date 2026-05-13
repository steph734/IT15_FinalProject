using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models;
using RealEstate.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RealEstate.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly ApplicationDBContext _db;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserService userService, ApplicationDBContext db, ILogger<AccountController> logger)
        {
            _userService = userService;
            _db = db;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // DEBUG: Test password hash
        [HttpGet("/Account/TestHash")]
        public IActionResult TestHash(string password = "Manager@123")
        {
            var hash = HashPassword(password);
            return Json(new { password, hash });
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                // Find user by email or username
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email || u.Username == model.Email);

                if (user != null && VerifyPassword(model.Password, user.PasswordHash))
                {
                    // Create claims for authentication
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("Username", user.Username)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation($"User {user.Email} logged in successfully.");

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Redirect based on user role
                    switch (user.Role.ToLower())
                    {
                        case "admin":
                        case "superadmin":
                            return RedirectToAction("Index", "Admin");
                        case "broker":
                            return RedirectToAction("Index", "Broker");
                        case "seller":
                            return RedirectToAction("Index", "Seller");
                        case "manager":
                            return RedirectToAction("Index", "Manager");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user already exists
                var existingUser = await _db.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email || u.Username == model.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "A user with this email or username already exists.");
                    return View(model);
                }

                // Create new user
                var user = new ApplicationUser
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Username = model.Username,
                    PhoneNumber = model.PhoneNumber,
                    Role = "Client", // Default role
                    PasswordHash = HashPassword(model.Password),
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"New user registered: {user.Email}");

                // Auto-login after registration
                await Login(new LoginViewModel
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = false
                });

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Account/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _db.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };

            return View(model);
        }

        // POST: /Account/CreateDemoUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDemoUser()
        {
            // Check if demo user already exists
            var existingDemo = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == "demo@estateflow.com");

            if (existingDemo == null)
            {
                // Create demo user
                var demoUser = new ApplicationUser
                {
                    FullName = "Demo User",
                    Email = "demo@estateflow.com",
                    Username = "demo",
                    PhoneNumber = "+1234567890",
                    Role = "Client",
                    PasswordHash = HashPassword("demo123"),
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(demoUser);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Demo user created successfully.");
                TempData["Success"] = "Demo account created! You can now login with demo@estateflow.com / demo123";
            }
            else
            {
                TempData["Info"] = "Demo account already exists.";
            }

            return RedirectToAction("Login");
        }

        // Helper methods
        private bool VerifyPassword(string password, string passwordHash)
        {
            // Simple password verification (in production, use proper hashing like BCrypt)
            return passwordHash == HashPassword(password);
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
    }

    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}
