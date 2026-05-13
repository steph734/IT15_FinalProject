using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models;
using System.Security.Claims;

namespace RealEstate.Controllers
{
    public class SupportTicketController : Controller
    {
        private readonly ApplicationDBContext _db;

        public SupportTicketController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: SupportTicket
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var tickets = _db.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedEmployee)
                .ThenInclude(e => e.User)
                .Include(t => t.RelatedProperty)
                .Include(t => t.Comments)
                .AsQueryable();

            // Filter based on role
            if (userRole == "Client")
            {
                tickets = tickets.Where(t => t.CustomerId.ToString() == userId);
            }
            else if (userRole == "Broker" || userRole == "Manager")
            {
                tickets = tickets.Where(t => t.AssignedTo.ToString() == userId || t.AssignedTo == null);
            }
            // SuperAdmin and Accounting can see all tickets

            return View(await tickets.OrderByDescending(t => t.CreatedAt).ToListAsync());
        }

        // GET: SupportTicket/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _db.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedEmployee)
                .Include(t => t.RelatedProperty)
                .Include(t => t.Comments.OrderBy(c => c.CreatedAt))
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        // GET: SupportTicket/Create
        public IActionResult Create()
        {
            ViewBag.Customers = _db.Customers.ToList();
            ViewBag.Employees = _db.Employees.Include(e => e.User).ToList();
            ViewBag.Properties = _db.Properties.ToList();
            return View();
        }

        // POST: SupportTicket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupportTicket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.CreatedAt = DateTime.UtcNow;
                ticket.Status = "Open";
                
                _db.SupportTickets.Add(ticket);
                await _db.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Customers = _db.Customers.ToList();
            ViewBag.Employees = _db.Employees.ToList();
            ViewBag.Properties = _db.Properties.ToList();
            return View(ticket);
        }

        // GET: SupportTicket/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _db.SupportTickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            ViewBag.Customers = _db.Customers.ToList();
            ViewBag.Employees = _db.Employees.Include(e => e.User).ToList();
            ViewBag.Properties = _db.Properties.ToList();
            return View(ticket);
        }

        // POST: SupportTicket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupportTicket ticket)
        {
            if (id != ticket.TicketId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTicket = await _db.SupportTickets.FindAsync(id);
                    if (existingTicket == null)
                        return NotFound();

                    existingTicket.Subject = ticket.Subject;
                    existingTicket.Description = ticket.Description;
                    existingTicket.Priority = ticket.Priority;
                    existingTicket.Status = ticket.Status;
                    existingTicket.Category = ticket.Category;
                    existingTicket.AssignedTo = ticket.AssignedTo;
                    existingTicket.DueDate = ticket.DueDate;
                    existingTicket.RelatedPropertyId = ticket.RelatedPropertyId;

                    if (ticket.Status == "Resolved" && !existingTicket.ResolvedAt.HasValue)
                        existingTicket.ResolvedAt = DateTime.UtcNow;

                    if (ticket.Status == "Closed" && !existingTicket.ClosedAt.HasValue)
                        existingTicket.ClosedAt = DateTime.UtcNow;

                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.TicketId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Customers = _db.Customers.ToList();
            ViewBag.Employees = _db.Employees.ToList();
            ViewBag.Properties = _db.Properties.ToList();
            return View(ticket);
        }

        // POST: SupportTicket/AddComment
        [HttpPost]
        public async Task<IActionResult> AddComment(int ticketId, string comment, bool isInternal = false)
        {
            var ticketComment = new TicketComment
            {
                TicketId = ticketId,
                Comment = comment,
                AuthorType = User.FindFirstValue(ClaimTypes.Role) ?? "Employee",
                IsInternal = isInternal,
                CreatedAt = DateTime.UtcNow
            };

            _db.TicketComments.Add(ticketComment);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        private bool TicketExists(int id)
        {
            return _db.SupportTickets.Any(e => e.TicketId == id);
        }
    }
}
