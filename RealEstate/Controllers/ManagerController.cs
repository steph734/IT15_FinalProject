using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models;
using RealEstate.Services;
using System.Text.Json;

namespace RealEstate.Controllers;

[Route("manager")]
[AuthorizeRole("Manager")]
public class ManagerController : Controller
{
    private readonly ApplicationDBContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly RentCastService _rentCastService;

    public ManagerController(ApplicationDBContext context, IWebHostEnvironment env, RentCastService rentCastService)
    {
        _context = context;
        _env = env;
        _rentCastService = rentCastService;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // DASHBOARD
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("")]
    public IActionResult Index() => RedirectToAction(nameof(Dashboard));

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;

        // Get current manager ID from session
        var managerId = HttpContext.Session.GetInt32("UserId") ?? 0;
        
        // Calculate Revenue Metrics (60/40 Split)
        var allSales = await _context.SaleTransactions.ToListAsync();
        
        // Total Manager Revenue (60% of commission pool) - Brokerage Earnings
        var totalManagerRevenue = allSales.Sum(s => s.BrokerEarnings);
        ViewData["TotalManagerRevenue"] = totalManagerRevenue;
        
        // Total Commission Pool (5% of sales)
        var totalCommissionPool = allSales.Sum(s => s.CommissionPool);
        ViewData["TotalCommissionPool"] = totalCommissionPool;
        
        // Agent Payouts (40% of commission pool) - Commission Expenses
        var totalAgentPayouts = allSales.Sum(s => s.AgentEarnings);
        ViewData["TotalAgentPayouts"] = totalAgentPayouts;
        
        // Today's Revenue (Manager's cut from today's sales)
        var today = DateTime.UtcNow.Date;
        var todaysSales = allSales.Where(s => s.TransactionDate.Date == today);
        var todaysManagerRevenue = todaysSales.Sum(s => s.BrokerEarnings);
        ViewData["TodaysManagerRevenue"] = todaysManagerRevenue;
        
        // Sales count for display
        ViewData["TotalSales"] = allSales.Count;
        ViewData["TodaysSalesCount"] = todaysSales.Count();
        
        // Get recent transactions with agent info for display
        var recentTransactions = await _context.SaleTransactions
            .Include(s => s.Agent)
            .OrderByDescending(s => s.TransactionDate)
            .Take(5)
            .ToListAsync();
        ViewBag.RecentTransactions = recentTransactions;

        var stats = new ManagerDashboardStats
        {
            TotalProperties = _context.Properties.Count(),
            PendingReviews = _context.Properties.Count(p => !p.IsApproved && !p.IsRejected),
            ApprovedProperties = _context.Properties.Count(p => p.IsApproved),
            TotalAgents = _context.Agents.Count()
        };

        return View("~/Views/Manager/Dashboard.cshtml", stats);
    }

    [HttpGet("coming-soon")]
    public IActionResult ComingSoon()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Manager/ComingSoon.cshtml");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // PROPERTIES LIST
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("properties")]
    public async Task<IActionResult> Properties()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;

        var properties = await _context.Properties
            .Include(p => p.Seller)
            .Include(p => p.Agent)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View("~/Views/Manager/Properties.cshtml", properties);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // PROPERTIES – Review and manage property listings
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("properties/pending")]
    public IActionResult PendingProperties()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        var properties = _context.Properties
            .AsNoTracking()
            .Where(p => !p.IsApproved && !p.IsRejected)
            .OrderByDescending(p => p.CreatedAt)
            .Take(200)
            .ToList();

        return View("~/Views/Manager/PendingProperties.cshtml", properties);
    }

    [HttpGet("sellers/listings")]
    public async Task<IActionResult> SellerListings(string status = "All")
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        ViewBag.CurrentStatus = status;

        var propertiesQuery = _context.Properties
            .Include(p => p.Seller)
            .AsNoTracking();

        // Calculate counts
        var allListings = await propertiesQuery.ToListAsync();
        ViewBag.CountAll = allListings.Count;
        ViewBag.CountPending = allListings.Count(p => p.Status == "PendingReview" || p.Status == "Inspection" || p.Status == "InspectionApproved" || p.Status == "InspectionContactSeller" || p.Status == "InspectionFailed");
        ViewBag.CountApproved = allListings.Count(p => p.IsApproved && p.Status != "Sold");
        ViewBag.CountRejected = allListings.Count(p => p.IsRejected);
        ViewBag.CountSold = allListings.Count(p => p.Status == "Sold");

        // Filter
        var filtered = status switch
        {
            "PendingReview" => allListings.Where(p => p.Status == "PendingReview" || p.Status == "Inspection" || p.Status == "InspectionApproved" || p.Status == "InspectionContactSeller" || p.Status == "InspectionFailed").ToList(),
            "Approved" => allListings.Where(p => p.IsApproved && p.Status != "Sold").ToList(),
            "Rejected" => allListings.Where(p => p.IsRejected).ToList(),
            "Sold" => allListings.Where(p => p.Status == "Sold").ToList(),
            _ => allListings
        };

        var ordered = filtered.OrderByDescending(p => p.CreatedAt).ToList();

        return View("~/Views/Manager/SellerListings.cshtml", ordered);
    }

    [HttpGet("properties/approved")]
    public IActionResult ApprovedProperties()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        var properties = _context.Properties
            .AsNoTracking()
            .Where(p => p.IsApproved)
            .OrderByDescending(p => p.DecisionAt)
            .Take(200)
            .ToList();

        return View("~/Views/Manager/ApprovedProperties.cshtml", properties);
    }

    [HttpGet("properties/{id:int}")]
    public async Task<IActionResult> PropertyDetail(int id)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        var property = await _context.Properties
            .AsNoTracking()
            .Include(p => p.Agent)
            .FirstOrDefaultAsync(p => p.PropertyId == id);

        if (property == null) return NotFound();

        if (property.Status == "Inspection")
        {
            ViewBag.InspectionItems = await _context.InspectionItems
                .Where(i => i.IsActive)
                .OrderBy(i => i.SortOrder)
                .ToListAsync();
        }

        // Agent is now loaded via Include
        ViewBag.AssignedAgent = property.Agent;

        return View("~/Views/Manager/PropertyDetail.cshtml", property);
    }

    [HttpPost("properties/{id:int}/mark-inspection")]
    public async Task<IActionResult> MarkForInspection(int id, [FromForm] int inspectionDays, [FromForm] string? notes)
    {
        var managerId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(managerId)) return RedirectToAction(nameof(SellerListings));

        var property = await _context.Properties.FindAsync(id);
        if (property == null) return NotFound();

        if (property.Status != "PendingReview")
        {
            TempData["ErrorMessage"] = "Property must be in Pending Review status.";
            return RedirectToAction(nameof(PropertyDetail), new { id });
        }

        if (inspectionDays <= 0) inspectionDays = 3;

        property.Status = "Inspection";
        property.InspectionScheduled = true;
        property.InspectionScheduledDate = DateTime.UtcNow.AddDays(inspectionDays);
        property.ExpectedTimeframeDays = inspectionDays;
        property.ManagerNotes = notes;

        // Create notification for seller only if the seller maps to a valid ApplicationUser
        if (property.SellerId.HasValue)
        {
            var sellerUser = await _context.Users.FindAsync(property.SellerId.Value);
            if (sellerUser != null)
            {
                var notification = new Notification
                {
                    UserId = sellerUser.UserId,
                    NotificationType = "InspectionScheduled",
                    Title = "Property Inspection Scheduled",
                    Message = $"Your property '{property.Title}' has been scheduled for inspection in {inspectionDays} days.",
                    RelatedEntityId = property.PropertyId,
                    RelatedEntityType = "Property",
                    IsRead = false,
                    CreatedAtUtc = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
            }
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Property '{property.Title}' marked for inspection.";
        return RedirectToAction(nameof(SellerListings));
    }

    [HttpPost("properties/{id:int}/save-inspection")]
    public async Task<IActionResult> SaveInspection(int id,
        [FromForm] bool DocumentsVerified,
        [FromForm] bool AreaVerified,
        [FromForm] bool LocationVerified,
        [FromForm] bool AmenitiesVerified,
        [FromForm] bool InspectionCompleted,
        [FromForm] string? InspectionResult,
        [FromForm] string? InspectionNotes)
    {
        var property = await _context.Properties.FindAsync(id);
        if (property == null) return NotFound();

        if (property.Status != "Inspection")
        {
            TempData["ErrorMessage"] = "Property must be under inspection.";
            return RedirectToAction(nameof(PropertyDetail), new { id });
        }

        property.DocumentsVerified = DocumentsVerified;
        property.AreaVerified = AreaVerified;
        property.LocationVerified = LocationVerified;
        property.AmenitiesVerified = AmenitiesVerified;
        property.InspectionCompleted = InspectionCompleted;
        property.InspectionResult = InspectionResult;
        property.InspectionNotes = InspectionNotes;

        if (InspectionCompleted)
        {
            property.InspectionCompletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Inspection data saved successfully.";
        return RedirectToAction(nameof(PropertyDetail), new { id });
    }

    [HttpPost("inspection-items/add")]
    public async Task<IActionResult> AddInspectionItem([FromBody] AddInspectionItemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Category) || string.IsNullOrWhiteSpace(request.Name))
            return Json(new { success = false, message = "Category and Name are required." });

        var maxSort = await _context.InspectionItems.MaxAsync(i => (int?)i.SortOrder) ?? 0;

        var item = new InspectionItem
        {
            Category = request.Category,
            Name = request.Name,
            Criteria = request.Criteria ?? "",
            Icon = "fa-clipboard",
            SortOrder = maxSort + 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.InspectionItems.Add(item);
        await _context.SaveChangesAsync();

        return Json(new { success = true, id = item.Id });
    }

    [HttpPost("inspection-items/{id:int}/delete")]
    public async Task<IActionResult> DeleteInspectionItem(int id)
    {
        var item = await _context.InspectionItems.FindAsync(id);
        if (item == null) return Json(new { success = false, message = "Item not found." });

        _context.InspectionItems.Remove(item);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPost("properties/{id:int}/complete-inspection")]
    public async Task<IActionResult> CompleteInspection(int id, [FromForm] string status)
    {
        var managerId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(managerId)) return RedirectToAction("Login", "Account");

        var property = await _context.Properties
            .Include(p => p.Seller)
            .FirstOrDefaultAsync(p => p.PropertyId == id);
            
        if (property == null) return NotFound();

        if (property.Status != "Inspection")
        {
            TempData["ErrorMessage"] = "Property is not under inspection.";
            return RedirectToAction(nameof(PropertyDetail), new { id });
        }

        property.Status = status;
        property.InspectionCompleted = true;
        property.InspectionCompletedAt = DateTime.UtcNow;

        if (status == "InspectionApproved")
        {
            property.InspectionResult = "Approved";
            // Notify seller
            if (property.Seller != null)
            {
                var notification = new Notification
                {
                    UserId = property.Seller.UserId,
                    Title = "Property Inspection Approved",
                    Message = $"Your property '{property.Title}' has passed inspection and is now pending final approval.",
                    CreatedAtUtc = DateTime.UtcNow,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
            }
            TempData["SuccessMessage"] = "Inspection approved. Ready for final approval.";
        }
        else if (status == "InspectionContactSeller")
        {
            property.InspectionResult = "For Contact";
            TempData["SuccessMessage"] = "Marked as For Contact. Please reach out to the seller.";
        }
        else if (status == "InspectionFailed")
        {
            property.InspectionResult = "Failed";
            TempData["ErrorMessage"] = "Inspection failed. Please proceed to reject the property.";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(PropertyDetail), new { id });
    }

    [HttpPost("properties/{id:int}/approve")]
    public async Task<IActionResult> ApproveProperty(int id,
        [FromForm] decimal finalPrice,
        [FromForm] string? paymentMethod,
        [FromForm] string? accountDetails,
        [FromForm] string? notes)
    {
        var managerId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(managerId))
        {
            TempData["ErrorMessage"] = "Manager not logged in.";
            return RedirectToAction(nameof(SellerListings));
        }

        var property = await _context.Properties.FindAsync(id);
        if (property == null) return NotFound();

        if (property.Status != "InspectionApproved")
        {
            TempData["ErrorMessage"] = "Property must pass inspection before it can be finally approved.";
            return RedirectToAction(nameof(PropertyDetail), new { id });
        }

        if (finalPrice <= 0)
        {
            TempData["ErrorMessage"] = "Please provide a valid final price.";
            return RedirectToAction(nameof(PropertyDetail), new { id });
        }

        // Format deal details
        var dealDetails = $"Payment Method: {paymentMethod ?? "N/A"}\nAccount Details: {accountDetails ?? "N/A"}\nNotes: {notes ?? "None"}";

        // Update property status
        property.IsApproved = true;
        property.IsRejected = false;
        property.FinalPrice = Math.Round(finalPrice, 2);
        property.DecisionAt = DateTime.UtcNow;
        property.Status = "Approved";
        property.FinalReviewNotes = dealDetails;

        // Fetch RentCast data for investment analysis
        try
        {
            var address = property.Location;
            var rentCastData = await _rentCastService.GetValuationAsync(address);

            if (rentCastData != null)
            {
                property.MarketValue = rentCastData.MarketValue;
                property.RentEstimate = rentCastData.RentEstimate;
                property.YieldScore = rentCastData.CalculateCapRate();
                property.ProfitabilityRating = rentCastData.GetProfitabilityRating();
                property.RentCastLastUpdated = DateTime.UtcNow;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RentCast] Error fetching data: {ex.Message}");
        }

        // Create notification for seller only if the seller maps to a valid ApplicationUser
        if (property.SellerId.HasValue)
        {
            var sellerUser = await _context.Users.FindAsync(property.SellerId.Value);
            if (sellerUser != null)
            {
                var notification = new Notification
                {
                    UserId = sellerUser.UserId,
                    NotificationType = "PropertyApproved",
                    Title = "Property Approved",
                    Message = $"Your property '{property.Title}' has been approved with a final price of ₱{finalPrice:N2}.",
                    RelatedEntityId = property.PropertyId,
                    RelatedEntityType = "Property",
                    IsRead = false,
                    CreatedAtUtc = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
            }
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Property '{property.Title}' approved successfully!";
        return RedirectToAction(nameof(SellerListings));
    }

    [HttpPost("properties/{id:int}/reject")]
    public async Task<IActionResult> RejectProperty(int id, [FromForm] string? reason)
    {
        var managerId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(managerId))
        {
            TempData["ErrorMessage"] = "Manager not logged in.";
            return RedirectToAction(nameof(SellerListings));
        }

        var property = await _context.Properties.FindAsync(id);
        if (property == null) return NotFound();

        if (property.IsApproved || property.IsRejected)
        {
            TempData["ErrorMessage"] = "Property has already been processed.";
            return RedirectToAction(nameof(PropertyDetail), new { id });
        }

        property.IsRejected = true;
        property.IsApproved = false;
        property.RejectionReason = reason ?? "Rejected by manager.";
        property.DecisionAt = DateTime.UtcNow;
        property.Status = "Rejected";

        // Create notification for seller only if the seller maps to a valid ApplicationUser
        if (property.SellerId.HasValue)
        {
            var sellerUser = await _context.Users.FindAsync(property.SellerId.Value);
            if (sellerUser != null)
            {
                var notification = new Notification
                {
                    UserId = sellerUser.UserId,
                    NotificationType = "PropertyRejected",
                    Title = "Property Rejected",
                    Message = $"Your property '{property.Title}' has been rejected. Reason: {property.RejectionReason}",
                    RelatedEntityId = property.PropertyId,
                    RelatedEntityType = "Property",
                    IsRead = false,
                    CreatedAtUtc = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
            }
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Property '{property.Title}' has been rejected.";
        return RedirectToAction(nameof(SellerListings));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // INVENTORY OVERVIEW
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("inventory-overview")]
    public IActionResult InventoryOverview()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        var properties = _context.Properties
            .AsNoTracking()
            .Where(p => p.IsApproved)
            .OrderByDescending(p => p.DecisionAt)
            .Take(100)
            .ToList();

        return View("~/Views/Manager/InventoryOverview.cshtml", properties);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // AGENTS
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("agents")]
    public IActionResult Agents()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        var agents = _context.Agents
            .AsNoTracking()
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .ToList();

        return View("~/Views/Manager/AgentList.cshtml", agents);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // COMMISSION RULES
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("commission-rules")]
    public IActionResult CommissionRules()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        var rules = _context.CommissionRules
            .AsNoTracking()
            .OrderBy(r => r.RuleId)
            .ToList();

        return View("~/Views/Manager/CommissionRules.cshtml", rules);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // AGENT ASSIGNMENT API
    // ══════════════════════════════════════════════════════════════════════════

    [HttpGet("get-agents")]
    public IActionResult GetAgents()
    {
        var agents = _context.Agents
            .AsNoTracking()
            .Select(a => new
            {
                agentId = a.Id,
                fullName = a.FirstName + " " + a.LastName
            })
            .OrderBy(a => a.fullName)
            .ToList();

        return Json(agents);
    }

    [HttpPost("assign-agent")]
    public IActionResult AssignAgent([FromBody] AssignAgentRequest request)
    {
        try
        {
            var property = _context.Properties.FirstOrDefault(p => p.PropertyId == request.PropertyId);
            if (property == null)
            {
                return Json(new { success = false, message = "Property not found" });
            }

            var agent = _context.Agents.FirstOrDefault(a => a.Id == request.AgentId);
            if (agent == null)
            {
                return Json(new { success = false, message = "Agent not found" });
            }

            // Update the property with the assigned agent (one agent can have many properties)
            property.AgentId = request.AgentId;

            _context.SaveChanges();

            return Json(new { success = true, message = "Agent assigned successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // COMMISSION & SALES
    // ══════════════════════════════════════════════════════════════════════════

    [HttpPost("commission-rules/save")]
    public IActionResult SaveCommissionRules([FromBody] CommissionRuleSaveRequest request)
    {
        try
        {
            // Update or create commission rules
            var existingRule = _context.CommissionRules.FirstOrDefault();
            if (existingRule == null)
            {
                existingRule = new CommissionRule();
                _context.CommissionRules.Add(existingRule);
            }
            
            existingRule.AgentSplitPercent = request.AgentSplit;
            existingRule.CompanySplitPercent = request.CompanySplit;
            existingRule.CommissionPercent = request.CommissionRate;
            existingRule.MinimumDealThreshold = request.MinimumThreshold;
            existingRule.UpdatedAt = DateTime.UtcNow;
            
            _context.SaveChanges();
            
            return Json(new { success = true, message = "Commission rules saved successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }

    [HttpPost("set-listing-type")]
    public async Task<IActionResult> SetListingType([FromBody] SetListingTypeRequest request)
    {
        try
        {
            var property = await _context.Properties.FindAsync(request.PropertyId);
            if (property == null)
            {
                return Json(new { success = false, message = "Property not found" });
            }

            // Update listing type
            if (Enum.TryParse<PropertyListingType>(request.ListingType, out var listingType))
            {
                property.ListingType = listingType;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Listing type updated successfully" });
            }
            
            return Json(new { success = false, message = "Invalid listing type" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }

    [HttpPost("process-sale")]
    public async Task<IActionResult> ProcessSale([FromBody] ProcessSaleRequest request)
    {
        try
        {
            // 1. Get the property
            var property = await _context.Properties
                .Include(p => p.Agent)
                .FirstOrDefaultAsync(p => p.PropertyId == request.PropertyId);
                
            if (property == null)
            {
                return Json(new { success = false, message = "Property not found" });
            }

            // 2. Get commission rules
            var rules = _context.CommissionRules.FirstOrDefault();
            var commissionRate = (rules?.CommissionPercent ?? 5m) / 100m;  // Default 5%
            var brokerSplitPercent = (rules?.CompanySplitPercent ?? 60m) / 100m;  // Default 60%
            var agentSplitPercent = (rules?.AgentSplitPercent ?? 40m) / 100m;  // Default 40%

            // 3. Calculate commission pool based on listing type
            decimal totalCommissionPool;
            if (property.ListingType == PropertyListingType.Buy)
            {
                // For Sale: 5% of final price
                totalCommissionPool = request.FinalPrice * 0.05m;
            }
            else
            {
                // For Rent: Multi-year calculation
                // 1st year = 100%, each additional year = +50%
                // Formula: Monthly Rent × (1 + (LeaseTerm - 1) × 0.5)
                int leaseTerm = request.LeaseTerm > 0 ? request.LeaseTerm : 1;
                decimal multiplier = 1m + ((leaseTerm - 1) * 0.5m); // 1, 1.5, 2, 2.5, 3...
                totalCommissionPool = request.FinalPrice * multiplier;
            }

            // 4. Run the Split Algorithm (60/40)
            decimal brokerCut = totalCommissionPool * brokerSplitPercent;
            decimal agentCut = totalCommissionPool * agentSplitPercent;

            // 5. Record the Transaction
            var sale = new SaleTransaction
            {
                PropertyId = request.PropertyId,
                AgentId = property.AgentId,
                ManagerId = HttpContext.Session.GetString("UserId") != null 
                    ? int.Parse(HttpContext.Session.GetString("UserId")!) 
                    : null,
                TotalContractPrice = request.FinalPrice,
                LeaseTerm = property.ListingType == PropertyListingType.Rent 
                    ? request.LeaseTerm 
                    : 1,  // For sales, always 1
                CommissionPool = totalCommissionPool,
                BrokerEarnings = brokerCut,
                AgentEarnings = agentCut,
                BrokerSplitPercent = brokerSplitPercent * 100,
                AgentSplitPercent = agentSplitPercent * 100,
                CommissionRate = commissionRate * 100,
                Status = "Pending",
                TransactionDate = DateTime.UtcNow,
                Notes = request.Notes
            };

            _context.SaleTransactions.Add(sale);

            // 6. Update Property Status - Set Ready for Listing based on type
            property.FinalPrice = request.FinalPrice;
            property.IsReadyForListing = true;
            property.ReadyForListingDate = DateTime.UtcNow;
            
            // Set status based on listing type
            property.Status = property.ListingType == PropertyListingType.Buy 
                ? "ReadyForSale" 
                : "ReadyForRent";

            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = property.ListingType == PropertyListingType.Buy 
                    ? "Property is now Ready for Sale" 
                    : "Property is now Ready for Rent",
                data = new {
                    saleId = sale.SaleId,
                    totalContractPrice = sale.TotalContractPrice,
                    commissionPool = sale.CommissionPool,
                    brokerEarnings = sale.BrokerEarnings,
                    agentEarnings = sale.AgentEarnings,
                    leaseTerm = sale.LeaseTerm,
                    status = property.Status
                }
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }

    [HttpGet("earnings-report")]
    public IActionResult EarningsReport()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        // Calculate totals
        var sales = _context.SaleTransactions
            .AsNoTracking()
            .Where(s => s.Status == "Approved" || s.Status == "Pending")
            .ToList();

        var viewModel = new EarningsReportViewModel
        {
            TotalSales = sales.Count,
            TotalSalesValue = sales.Sum(s => s.TotalContractPrice),
            TotalBrokerEarnings = sales.Sum(s => s.BrokerEarnings),
            TotalAgentPayouts = sales.Sum(s => s.AgentEarnings),
            TotalCommissionPool = sales.Sum(s => s.CommissionPool),
            RecentSales = sales.OrderByDescending(s => s.TransactionDate).Take(10).ToList()
        };

        return View("~/Views/Manager/EarningsReport.cshtml", viewModel);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // LOGOUT
    // ══════════════════════════════════════════════════════════════════════════

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Admin");
    }
}

// ViewModel for Dashboard Stats
public class ManagerDashboardStats
{
    public int TotalProperties { get; set; }
    public int PendingReviews { get; set; }
    public int ApprovedProperties { get; set; }
    public int TotalAgents { get; set; }
}

public class AssignAgentRequest
{
    public int PropertyId { get; set; }
    public int AgentId { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class AddInspectionItemRequest
{
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Criteria { get; set; }
}

public class SetListingTypeRequest
{
    public int PropertyId { get; set; }
    public string ListingType { get; set; } = string.Empty;  // "Buy" or "Rent"
}

public class CommissionRuleSaveRequest
{
    public decimal AgentSplit { get; set; }
    public decimal CompanySplit { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal MinimumThreshold { get; set; }
}

public class ProcessSaleRequest
{
    public int PropertyId { get; set; }
    public decimal FinalPrice { get; set; }
    public int LeaseTerm { get; set; } = 1;  // For rentals: 1, 2, 3 years etc.
    public string? Notes { get; set; }
}

public class EarningsReportViewModel
{
    public int TotalSales { get; set; }
    public decimal TotalSalesValue { get; set; }
    public decimal TotalBrokerEarnings { get; set; }
    public decimal TotalAgentPayouts { get; set; }
    public decimal TotalCommissionPool { get; set; }
    public List<SaleTransaction> RecentSales { get; set; } = new List<SaleTransaction>();
}

