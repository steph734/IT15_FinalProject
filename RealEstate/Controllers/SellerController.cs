using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models;
using RealEstate.Services;
using System.Text.Json;
using System.Text.RegularExpressions;
using RealEstate.Models.Seller;

namespace RealEstate.Controllers;

[Route("seller")]
[AuthorizeRole("Seller")]
public class SellerController : Controller
{
    private readonly ApplicationDBContext _context;
    private readonly IWebHostEnvironment _env;

    public SellerController(ApplicationDBContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // ── Dashboard ─────────────────────────────────────────────────────────────

    [HttpGet("")]
    public IActionResult Index() => RedirectToAction(nameof(Dashboard));

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var sellerId = GetSellerId();
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;

        var properties = _context.Properties
            .AsNoTracking()
            .Where(p => p.SellerId == sellerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        var vm = new SellerDashboardViewModel
        {
            TotalListings = properties.Count,
            PendingListings = properties.Count(p => p.ReviewStatus == "NotStarted" || p.ReviewStatus == "CheckingDocuments"),
            ApprovedListings = properties.Count(p => p.IsApproved),
            RejectedListings = properties.Count(p => p.IsRejected),
            SoldListings = properties.Count(p => p.Status == "Sold"),
            TotalEarnings = properties.Where(p => p.Status == "Sold").Sum(p => p.FinalPrice ?? p.BasePrice),
            RecentListings = properties.Take(5).ToList()
        };

        return View("~/Views/Seller/Dashboard.cshtml", vm);
    }

    // ── My Listings ───────────────────────────────────────────────────────────

    [HttpGet("my-listings")]
    public IActionResult MyListings(string? status = null, bool showDeleted = false, bool showArchived = false)
    {
        var sellerId = GetSellerId();
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        // Ensure sellerId is valid
        if (sellerId == 0)
        {
            TempData["ErrorMessage"] = "Unable to identify seller. Please log in again.";
            return RedirectToAction("Login", "Admin");
        }

        var query = _context.Properties
            .AsNoTracking()
            .Where(p => p.SellerId == sellerId);

        // Filter by view mode
        if (showDeleted)
        {
            query = query.Where(p => p.IsDeleted);
        }
        else if (showArchived)
        {
            query = query.Where(p => p.IsArchived && !p.IsDeleted);
        }
        else
        {
            // Active view: show non-archived and non-deleted
            query = query.Where(p => !p.IsArchived && !p.IsDeleted);
        }

        // Filter by status (only for active view)
        if (!string.IsNullOrWhiteSpace(status) && !showDeleted && !showArchived)
            query = query.Where(p => p.Status == status);

        var properties = query.OrderByDescending(p => p.CreatedAt).ToList();

        ViewBag.StatusFilter = status;
        ViewBag.ShowDeleted = showDeleted;
        ViewBag.ShowArchived = showArchived;
        return View("~/Views/Seller/MyListings.cshtml", properties);
    }

    // ── Submit Listing ────────────────────────────────────────────────────────

    [HttpGet("submit")]
    public IActionResult Submit()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        return View("~/Views/Seller/SubmitListing.cshtml");
    }

    [HttpPost("submit")]
    [RequestSizeLimit(50_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 50_000_000)]
    public async Task<IActionResult> SubmitListing(
        [FromForm] string title,
        [FromForm] string location,
        [FromForm] string propertyType,
        [FromForm] int bedrooms,
        [FromForm] int bathrooms,
        [FromForm] int parkingSlots,
        [FromForm] int sqft,
        [FromForm] string description,
        [FromForm] decimal suggestedPrice,
        [FromForm] string? latitude,
        [FromForm] string? longitude,
        [FromForm] IFormFile? coverImage,
        [FromForm] IFormFile? document1,
        [FromForm] IFormFile? document2,
        [FromForm] IFormFile? document3)
    {
        var sellerId = GetSellerId();
        var sellerName = HttpContext.Session.GetString("UserName") ?? "Seller";
        ViewData["UserName"] = sellerName;

        title = (title ?? "").Trim();
        location = (location ?? "").Trim();

        if (title.Length == 0 || location.Length == 0 || suggestedPrice <= 0)
        {
            TempData["ErrorMessage"] = "Title, location, and a valid suggested price are required.";
            return RedirectToAction(nameof(Submit));
        }

        // Parse coordinates
        double? lat = null, lng = null;
        if (!string.IsNullOrWhiteSpace(latitude) && double.TryParse(latitude, out var latVal))
            lat = latVal;
        if (!string.IsNullOrWhiteSpace(longitude) && double.TryParse(longitude, out var lngVal))
            lng = lngVal;

        // Save documents
        var docUrls = new List<string>();
        var docFiles = new[] { document1, document2, document3 };
        foreach (var f in docFiles)
        {
            var url = await SaveUploadAsync(f, "seller-docs", sellerId);
            if (url != null) docUrls.Add(url);
        }

        // Save cover image
        var coverUrl = await SaveUploadAsync(coverImage, "seller-covers", sellerId);

        var property = new Property
        {
            SellerId = sellerId,
            Title = title,
            Location = location,
            PropertyType = (propertyType ?? "House").Trim(),
            Bedrooms = Math.Max(0, bedrooms),
            Bathrooms = Math.Max(0, bathrooms),
            ParkingSlots = Math.Max(0, parkingSlots),
            Sqft = Math.Max(0, sqft),
            Description = (description ?? "").Trim(),
            SuggestedPrice = suggestedPrice,
            BasePrice = suggestedPrice,
            Latitude = lat,
            Longitude = lng,
            DocumentUrlsJson = JsonSerializer.Serialize(docUrls),
            CoverImage = coverUrl,
            ReviewStatus = "NotStarted",
            Status = "PendingReview",
            CreatedAt = DateTime.UtcNow
        };

        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Your property has been submitted for review!";
        return RedirectToAction(nameof(MyListings));
    }

    // ── Listing Details ───────────────────────────────────────────────────────

    [HttpGet("listing/{id:int}")]
    public IActionResult ListingDetails(int id)
    {
        var sellerId = GetSellerId();
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        var property = _context.Properties
            .AsNoTracking()
            .FirstOrDefault(p => p.PropertyId == id && p.SellerId == sellerId);

        if (property == null) return NotFound();

        return View("~/Views/Seller/ListingDetails.cshtml", property);
    }

    // ── Edit Listing ──────────────────────────────────────────────────────────

    [HttpGet("listing/{id:int}/edit")]
    public IActionResult EditListing(int id)
    {
        var sellerId = GetSellerId();
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        var property = _context.Properties
            .FirstOrDefault(p => p.PropertyId == id && p.SellerId == sellerId);

        if (property == null)
        {
            TempData["ErrorMessage"] = "Listing not found.";
            return RedirectToAction(nameof(MyListings));
        }

        // Only allow editing if not yet approved
        if (property.IsApproved)
        {
            TempData["ErrorMessage"] = "Cannot edit approved listings.";
            return RedirectToAction(nameof(ListingDetails), new { id = id });
        }

        return View("~/Views/Seller/SubmitListing.cshtml", property);
    }

    [HttpPost("listing/{id:int}/update")]
    [RequestSizeLimit(50_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 50_000_000)]
    public async Task<IActionResult> UpdateListing(
        int id,
        [FromForm] string title,
        [FromForm] string location,
        [FromForm] string propertyType,
        [FromForm] int bedrooms,
        [FromForm] int bathrooms,
        [FromForm] int parkingSlots,
        [FromForm] int sqft,
        [FromForm] string description,
        [FromForm] decimal suggestedPrice,
        [FromForm] string? latitude,
        [FromForm] string? longitude,
        [FromForm] IFormFile? coverImage,
        [FromForm] IFormFile? document1,
        [FromForm] IFormFile? document2,
        [FromForm] IFormFile? document3)
    {
        var sellerId = GetSellerId();
        var sellerName = HttpContext.Session.GetString("UserName") ?? "Seller";
        ViewData["UserName"] = sellerName;

        var property = _context.Properties
            .FirstOrDefault(p => p.PropertyId == id && p.SellerId == sellerId);

        if (property == null)
        {
            TempData["ErrorMessage"] = "Listing not found.";
            return RedirectToAction(nameof(MyListings));
        }

        // Only allow editing if not yet approved
        if (property.IsApproved)
        {
            TempData["ErrorMessage"] = "Cannot edit approved listings.";
            return RedirectToAction(nameof(ListingDetails), new { id = id });
        }

        title = (title ?? "").Trim();
        location = (location ?? "").Trim();

        if (title.Length == 0 || location.Length == 0 || suggestedPrice <= 0)
        {
            TempData["ErrorMessage"] = "Title, location, and a valid suggested price are required.";
            return RedirectToAction(nameof(EditListing), new { id = id });
        }

        // Update fields
        property.Title = title;
        property.Location = location;
        property.PropertyType = (propertyType ?? "House").Trim();
        property.Bedrooms = Math.Max(0, bedrooms);
        property.Bathrooms = Math.Max(0, bathrooms);
        property.ParkingSlots = Math.Max(0, parkingSlots);
        property.Sqft = Math.Max(0, sqft);
        property.Description = (description ?? "").Trim();
        property.SuggestedPrice = suggestedPrice;
        property.BasePrice = suggestedPrice;

        // Update coordinates if provided
        if (!string.IsNullOrWhiteSpace(latitude) && double.TryParse(latitude, out var lat))
            property.Latitude = lat;
        if (!string.IsNullOrWhiteSpace(longitude) && double.TryParse(longitude, out var lng))
            property.Longitude = lng;

        // Save new cover image if provided
        var coverUrl = await SaveUploadAsync(coverImage, "seller-covers", sellerId);
        if (coverUrl != null)
        {
            property.CoverImage = coverUrl;
        }

        // Add new documents if provided
        var existingDocs = JsonSerializer.Deserialize<List<string>>(property.DocumentUrlsJson ?? "[]");
        var docFiles = new[] { document1, document2, document3 };
        foreach (var f in docFiles)
        {
            var url = await SaveUploadAsync(f, "seller-docs", sellerId);
            if (url != null) existingDocs.Add(url);
        }
        property.DocumentUrlsJson = JsonSerializer.Serialize(existingDocs);

        // Reset status if it was rejected
        if (property.IsRejected)
        {
            property.IsRejected = false;
            property.RejectionReason = null;
            property.ReviewStatus = "NotStarted";
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Listing updated successfully!";
        return RedirectToAction(nameof(ListingDetails), new { id = id });
    }

    // ── Archive Listing ─────────────────────────────────────────────────────────

    [HttpGet("listing/{id:int}/archive")]
    public IActionResult ArchiveListing(int id)
    {
        var sellerId = GetSellerId();
        var property = _context.Properties
            .FirstOrDefault(p => p.PropertyId == id && p.SellerId == sellerId);

        if (property == null)
        {
            TempData["ErrorMessage"] = "Listing not found.";
            return RedirectToAction(nameof(MyListings));
        }

        property.IsArchived = true;
        property.ArchivedAt = DateTime.UtcNow;
        property.Status = "Archived";
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Listing has been archived successfully!";
        return RedirectToAction(nameof(MyListings));
    }

    [HttpPost("listing/{id:int}/archive")]
    public IActionResult ArchiveListingPost(int id)
    {
        return ArchiveListing(id);
    }

    [HttpPost("listing/{id:int}/unarchive")]
    public IActionResult UnarchiveListing(int id)
    {
        var sellerId = GetSellerId();
        var property = _context.Properties
            .FirstOrDefault(p => p.PropertyId == id && p.SellerId == sellerId);

        if (property == null)
        {
            TempData["ErrorMessage"] = "Listing not found.";
            return RedirectToAction(nameof(MyListings));
        }

        property.IsArchived = false;
        property.ArchivedAt = null;
        property.Status = "Pending";
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Listing has been unarchived successfully!";
        return RedirectToAction(nameof(MyListings));
    }

    // ── Soft Delete Listing ─────────────────────────────────────────────────────

    [HttpGet("listing/{id:int}/soft-delete")]
    public IActionResult SoftDeleteListing(int id)
    {
        var sellerId = GetSellerId();
        var property = _context.Properties
            .FirstOrDefault(p => p.PropertyId == id && p.SellerId == sellerId);

        if (property == null)
        {
            TempData["ErrorMessage"] = "Listing not found.";
            return RedirectToAction(nameof(MyListings));
        }

        // Only allow soft delete if not sold
        if (property.IsSold)
        {
            TempData["ErrorMessage"] = "Cannot soft delete sold listings.";
            return RedirectToAction(nameof(ListingDetails), new { id = id });
        }

        property.IsDeleted = true;
        property.DeletedAt = DateTime.UtcNow;
        property.Status = "Deleted";
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Listing has been moved to trash successfully!";
        return RedirectToAction(nameof(MyListings));
    }

    [HttpPost("listing/{id:int}/soft-delete")]
    public IActionResult SoftDeleteListingPost(int id)
    {
        return SoftDeleteListing(id);
    }

    // ── Restore Listing ─────────────────────────────────────────────────────────

    [HttpPost("listing/{id:int}/restore")]
    public IActionResult RestoreListing(int id)
    {
        var sellerId = GetSellerId();
        var property = _context.Properties
            .FirstOrDefault(p => p.PropertyId == id && p.SellerId == sellerId);

        if (property == null)
        {
            TempData["ErrorMessage"] = "Listing not found.";
            return RedirectToAction(nameof(MyListings));
        }

        property.IsDeleted = false;
        property.DeletedAt = null;
        property.Status = "Pending";
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Listing has been restored successfully!";
        return RedirectToAction(nameof(MyListings));
    }

    // ── Permanent Delete Listing ─────────────────────────────────────────────────

    [HttpPost("listing/{id:int}/permanent-delete")]
    public IActionResult PermanentDeleteListing(int id)
    {
        var sellerId = GetSellerId();
        var property = _context.Properties
            .FirstOrDefault(p => p.PropertyId == id && p.SellerId == sellerId);

        if (property == null)
        {
            TempData["ErrorMessage"] = "Listing not found.";
            return RedirectToAction(nameof(MyListings));
        }

        _context.Properties.Remove(property);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Listing has been permanently deleted!";
        return RedirectToAction(nameof(MyListings));
    }

    // ── Delete Listing (Deprecated - use Soft Delete) ────────────────────────────

    [HttpPost("listing/{id:int}/delete")]
    public IActionResult DeleteListing(int id)
    {
        var sellerId = GetSellerId();
        var property = _context.Properties
            .FirstOrDefault(p => p.PropertyId == id && p.SellerId == sellerId);

        if (property == null)
        {
            TempData["ErrorMessage"] = "Listing not found.";
            return RedirectToAction(nameof(MyListings));
        }

        // Only allow deletion if not sold
        if (property.IsSold)
        {
            TempData["ErrorMessage"] = "Cannot delete sold listings.";
            return RedirectToAction(nameof(ListingDetails), new { id = id });
        }

        _context.Properties.Remove(property);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Listing has been deleted successfully!";
        return RedirectToAction(nameof(MyListings));
    }

    // ── Notifications ─────────────────────────────────────────────────────────

    [HttpGet("notifications")]
    public IActionResult Notifications()
    {
        var sellerId = GetSellerId();
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        var notifications = _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == sellerId)
            .OrderByDescending(n => n.CreatedAtUtc)
            .Take(50)
            .ToList();

        ViewBag.UnreadCount = notifications.Count(n => !n.IsRead);

        return View("~/Views/Seller/Notifications.cshtml", notifications);
    }

    [HttpPost("notifications/{id:int}/mark-read")]
    public IActionResult MarkNotificationRead(int id)
    {
        var sellerId = GetSellerId();
        var notification = _context.Notifications.FirstOrDefault(n => n.NotificationId == id && n.UserId == sellerId);

        if (notification == null) return NotFound();

        notification.IsRead = true;
        notification.ReadAtUtc = DateTime.UtcNow;
        _context.SaveChanges();

        return Ok();
    }

    [HttpPost("notifications/mark-all-read")]
    public IActionResult MarkAllNotificationsRead()
    {
        var sellerId = GetSellerId();
        var notifications = _context.Notifications.Where(n => n.UserId == sellerId && !n.IsRead).ToList();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAtUtc = DateTime.UtcNow;
        }

        _context.SaveChanges();
        return RedirectToAction(nameof(Notifications));
    }

    // ── Profile ───────────────────────────────────────────────────────────────

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        return View("~/Views/Seller/Profile.cshtml");
    }

    // ── Support ───────────────────────────────────────────────────────────────

    [HttpGet("support")]
    public IActionResult Support()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        return View("~/Views/Seller/Support.cshtml");
    }

    // ── Logout ────────────────────────────────────────────────────────────────

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Admin");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private int GetSellerId()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (!int.TryParse(userIdStr, out int userId) || userId == 0) return 0;

        var seller = _context.Sellers.FirstOrDefault(s => s.UserId == userId);
        if (seller == null)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                seller = new PropertySeller
                {
                    UserId = userId,
                    SellerName = user.FullName ?? user.Email ?? "Unknown",
                    Rating = 0,
                    IdentityVerified = false
                };
                _context.Sellers.Add(seller);
                _context.SaveChanges();
            }
            else
            {
                return 0;
            }
        }
        return seller.SellerId;
    }

    private async Task<string?> SaveUploadAsync(IFormFile? file, string folder, int sellerId)
    {
        if (file == null || file.Length == 0) return null;

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".webp", ".gif" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext)) return null;

        var root = Path.Combine(
            _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
            "uploads", folder);
        Directory.CreateDirectory(root);

        var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssff");
        var safe = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^a-zA-Z0-9\-_]+", "-");
        var fileName = $"seller{sellerId}_{stamp}_{safe}{ext}";
        var path = Path.Combine(root, fileName);

        await using var fs = System.IO.File.Create(path);
        await file.CopyToAsync(fs);

        return $"/uploads/{folder}/{fileName}";
    }
}

