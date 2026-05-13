using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models;
using System.Security.Claims;

namespace RealEstate.Controllers
{
    public class MarketingCampaignController : Controller
    {
        private readonly ApplicationDBContext _db;

        public MarketingCampaignController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: MarketingCampaign
        public async Task<IActionResult> Index()
        {
            var campaigns = await _db.MarketingCampaigns
                .Include(c => c.Creator)
                .Include(c => c.Recipients)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(campaigns);
        }

        // GET: MarketingCampaign/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var campaign = await _db.MarketingCampaigns
                .Include(c => c.Creator)
                .Include(c => c.Recipients)
                .Include(c => c.Activities)
                .FirstOrDefaultAsync(c => c.CampaignId == id);

            if (campaign == null)
                return NotFound();

            return View(campaign);
        }

        // GET: MarketingCampaign/Create
        public IActionResult Create()
        {
            ViewBag.Segments = _db.CustomerSegments.Where(s => s.IsActive).ToList();
            return View();
        }

        // POST: MarketingCampaign/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MarketingCampaign campaign)
        {
            if (ModelState.IsValid)
            {
                campaign.CreatedAt = DateTime.UtcNow;
                campaign.Status = "Draft";
                
                _db.MarketingCampaigns.Add(campaign);
                await _db.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Segments = _db.CustomerSegments.Where(s => s.IsActive).ToList();
            return View(campaign);
        }

        // GET: MarketingCampaign/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var campaign = await _db.MarketingCampaigns.FindAsync(id);
            if (campaign == null)
                return NotFound();

            ViewBag.Segments = _db.CustomerSegments.Where(s => s.IsActive).ToList();
            return View(campaign);
        }

        // POST: MarketingCampaign/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MarketingCampaign campaign)
        {
            if (id != campaign.CampaignId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCampaign = await _db.MarketingCampaigns.FindAsync(id);
                    if (existingCampaign == null)
                        return NotFound();

                    existingCampaign.Name = campaign.Name;
                    existingCampaign.Description = campaign.Description;
                    existingCampaign.CampaignType = campaign.CampaignType;
                    existingCampaign.Status = campaign.Status;
                    existingCampaign.StartDate = campaign.StartDate;
                    existingCampaign.EndDate = campaign.EndDate;
                    existingCampaign.Budget = campaign.Budget;
                    existingCampaign.TargetSegment = campaign.TargetSegment;
                    existingCampaign.ModifiedAt = DateTime.UtcNow;

                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CampaignExists(campaign.CampaignId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Segments = _db.CustomerSegments.Where(s => s.IsActive).ToList();
            return View(campaign);
        }

        // POST: MarketingCampaign/Launch/5
        [HttpPost]
        public async Task<IActionResult> Launch(int id)
        {
            var campaign = await _db.MarketingCampaigns.FindAsync(id);
            if (campaign == null)
                return NotFound();

            campaign.Status = "Active";
            campaign.StartDate = DateTime.UtcNow;
            campaign.ModifiedAt = DateTime.UtcNow;

            // Add recipients based on target segment
            if (!string.IsNullOrEmpty(campaign.TargetSegment))
            {
                var segment = await _db.CustomerSegments
                    .Include(s => s.Memberships)
                    .ThenInclude(m => m.Customer)
                    .FirstOrDefaultAsync(s => s.Name == campaign.TargetSegment);

                if (segment != null)
                {
                    foreach (var membership in segment.Memberships)
                    {
                        var recipient = new CampaignRecipient
                        {
                            CampaignId = campaign.CampaignId,
                            CustomerId = membership.CustomerId,
                            Email = membership.Customer.Email,
                            PhoneNumber = membership.Customer.Phone,
                            Status = "Pending"
                        };
                        _db.CampaignRecipients.Add(recipient);
                    }
                    campaign.TargetAudienceCount = segment.Memberships.Count;
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CampaignExists(int id)
        {
            return _db.MarketingCampaigns.Any(e => e.CampaignId == id);
        }
    }
}
