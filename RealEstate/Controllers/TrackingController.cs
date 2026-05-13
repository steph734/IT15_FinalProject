using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models;
using RealEstate.Services;

namespace RealEstate.Controllers;

public class TrackingController : Controller
{
    private readonly ApplicationDBContext _db;

    public TrackingController(ApplicationDBContext db)
    {
        _db = db;
    }

    // ═══════════════════════════════════════════════════════
    // 👤 CUSTOMER FLOW
    // ═══════════════════════════════════════════════════════

    // Customer: Start tracking after scheduling viewing
    [HttpGet("tracking/customer/{appointmentId}")]
    public IActionResult CustomerTracking(int appointmentId)
    {
        var appointment = _db.ViewingAppointments
            .Include(a => a.Property)
            .FirstOrDefault(a => a.Id == appointmentId);

        if (appointment == null)
        {
            TempData["ErrorMessage"] = "Viewing appointment not found.";
            return RedirectToAction("Index", "Home");
        }

        // Check if there's an active tracking session
        var activeSession = _db.TrackingSessions
            .FirstOrDefault(s => s.ViewingAppointmentId == appointmentId && s.Status == TrackingSessionStatus.Active);

        ViewBag.Appointment = appointment;
        ViewBag.Property = appointment.Property;
        ViewBag.HasActiveSession = activeSession != null;
        ViewBag.SessionId = activeSession?.Id;
        
        return View();
    }

    // Customer: Share location
    [HttpPost("tracking/customer/{appointmentId}/share-location")]
    public IActionResult ShareCustomerLocation(int appointmentId, [FromBody] LocationData location)
    {
        try
        {
            var session = _db.TrackingSessions
                .FirstOrDefault(s => s.ViewingAppointmentId == appointmentId && s.Status == TrackingSessionStatus.Active);

            if (session == null)
            {
                // Create new tracking session
                var appointment = _db.ViewingAppointments
                    .Include(a => a.Property)
                    .FirstOrDefault(a => a.Id == appointmentId);

                if (appointment == null || appointment.Property == null)
                {
                    return Json(new { success = false, message = "Appointment or property not found" });
                }

                session = new TrackingSession
                {
                    ViewingAppointmentId = appointmentId,
                    PropertyId = appointment.PropertyId,
                    CustomerName = appointment.CustomerName,
                    CustomerEmail = appointment.CustomerEmail,
                    CustomerPhone = appointment.CustomerPhone,
                    PropertyLatitude = appointment.Property.Latitude ?? 0,
                    PropertyLongitude = appointment.Property.Longitude ?? 0,
                    CustomerLatitude = location.Latitude,
                    CustomerLongitude = location.Longitude,
                    Status = TrackingSessionStatus.Active,
                    StartedAt = DateTime.UtcNow
                };

                _db.TrackingSessions.Add(session);
            }
            else
            {
                session.CustomerLatitude = location.Latitude;
                session.CustomerLongitude = location.Longitude;
                session.LastUpdated = DateTime.UtcNow;
            }

            _db.SaveChanges();

            return Json(new { success = true, sessionId = session.Id });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // Customer: Get agent location (for real-time updates)
    [HttpGet("tracking/customer/{appointmentId}/agent-location")]
    public IActionResult GetAgentLocation(int appointmentId)
    {
        var session = _db.TrackingSessions
            .FirstOrDefault(s => s.ViewingAppointmentId == appointmentId && s.Status == TrackingSessionStatus.Active);

        if (session == null)
        {
            return Json(new { success = false, message = "No active session" });
        }

        return Json(new
        {
            success = true,
            agentLat = session.AgentLatitude,
            agentLng = session.AgentLongitude,
            agentName = session.Agent?.Name ?? session.Agent?.DisplayName ?? "Agent",
            status = session.Status.ToString()
        });
    }

    // ═══════════════════════════════════════════════════════
    // 🧑‍💻 AGENT FLOW
    // ═══════════════════════════════════════════════════════

    // Agent: View assigned viewings
    [HttpGet("tracking/agent/dashboard")]
    public IActionResult AgentDashboard()
    {
        var viewings = _db.ViewingAppointments
            .Include(a => a.Property)
            .Where(a => a.Status == AppointmentStatus.Scheduled)
            .OrderByDescending(a => a.WhenUtc)
            .ToList();

        var activeSessions = _db.TrackingSessions
            .Include(s => s.ViewingAppointment)
            .ThenInclude(va => va.Property)
            .Where(s => s.Status == TrackingSessionStatus.Active)
            .ToList();

        ViewBag.Viewings = viewings;
        ViewBag.ActiveSessions = activeSessions;
        return View();
    }

    // Agent: Accept viewing request
    [HttpPost("tracking/agent/{appointmentId}/accept")]
    public IActionResult AcceptViewing(int appointmentId)
    {
        var appointment = _db.ViewingAppointments.Find(appointmentId);
        if (appointment == null)
        {
            return Json(new { success = false, message = "Appointment not found" });
        }

        // Create or activate tracking session
        var session = _db.TrackingSessions
            .FirstOrDefault(s => s.ViewingAppointmentId == appointmentId);

        if (session == null)
        {
            var property = _db.Properties.Find(appointment.PropertyId);
            session = new TrackingSession
            {
                ViewingAppointmentId = appointmentId,
                PropertyId = appointment.PropertyId,
                CustomerName = appointment.CustomerName,
                CustomerEmail = appointment.CustomerEmail,
                CustomerPhone = appointment.CustomerPhone,
                PropertyLatitude = property?.Latitude ?? 0,
                PropertyLongitude = property?.Longitude ?? 0,
                Status = TrackingSessionStatus.Active,
                StartedAt = DateTime.UtcNow
            };
            _db.TrackingSessions.Add(session);
        }
        else
        {
            session.Status = TrackingSessionStatus.Active;
            session.StartedAt = DateTime.UtcNow;
        }

        _db.SaveChanges();

        return Json(new { success = true, sessionId = session.Id });
    }

    // Agent: Start tracking session
    [HttpGet("tracking/agent/{appointmentId}")]
    public IActionResult AgentTracking(int appointmentId)
    {
        var appointment = _db.ViewingAppointments
            .Include(a => a.Property)
            .FirstOrDefault(a => a.Id == appointmentId);

        if (appointment == null)
        {
            TempData["ErrorMessage"] = "Viewing appointment not found.";
            return RedirectToAction("AgentDashboard");
        }

        var session = _db.TrackingSessions
            .FirstOrDefault(s => s.ViewingAppointmentId == appointmentId && s.Status == TrackingSessionStatus.Active);

        ViewBag.Appointment = appointment;
        ViewBag.Property = appointment.Property;
        ViewBag.Session = session;
        ViewBag.SessionId = session?.Id;
        
        return View();
    }

    // Agent: Share location
    [HttpPost("tracking/agent/{appointmentId}/share-location")]
    public IActionResult ShareAgentLocation(int appointmentId, [FromBody] LocationData location)
    {
        try
        {
            var session = _db.TrackingSessions
                .FirstOrDefault(s => s.ViewingAppointmentId == appointmentId && s.Status == TrackingSessionStatus.Active);

            if (session == null)
            {
                return Json(new { success = false, message = "No active tracking session" });
            }

            session.AgentLatitude = location.Latitude;
            session.AgentLongitude = location.Longitude;
            session.LastUpdated = DateTime.UtcNow;
            _db.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // Agent: End tracking session
    [HttpPost("tracking/agent/{appointmentId}/end-session")]
    public IActionResult EndAgentSession(int appointmentId, [FromBody] EndSessionData data)
    {
        try
        {
            var session = _db.TrackingSessions
                .FirstOrDefault(s => s.ViewingAppointmentId == appointmentId && s.Status == TrackingSessionStatus.Active);

            if (session == null)
            {
                return Json(new { success = false, message = "No active tracking session" });
            }

            session.Status = TrackingSessionStatus.Completed;
            session.EndedAt = DateTime.UtcNow;
            session.Notes = data.Notes;
            _db.SaveChanges();

            // Update viewing appointment status
            var appointment = _db.ViewingAppointments.Find(appointmentId);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Completed;
                _db.SaveChanges();
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ═══════════════════════════════════════════════════════
    // 🧑‍💼 MANAGER FLOW (MONITORING ONLY)
    // ═══════════════════════════════════════════════════════

    // Manager: View all tracking sessions
    [HttpGet("tracking/manager/dashboard")]
    public IActionResult ManagerDashboard()
    {
        var activeSessions = _db.TrackingSessions
            .Include(s => s.Property)
            .Include(s => s.Agent)
            .Include(s => s.ViewingAppointment)
            .Where(s => s.Status == TrackingSessionStatus.Active)
            .OrderByDescending(s => s.StartedAt)
            .ToList();

        var completedSessions = _db.TrackingSessions
            .Include(s => s.Property)
            .Include(s => s.Agent)
            .Include(s => s.ViewingAppointment)
            .Where(s => s.Status == TrackingSessionStatus.Completed)
            .OrderByDescending(s => s.EndedAt)
            .Take(20)
            .ToList();

        var pendingViewings = _db.ViewingAppointments
            .Include(a => a.Property)
            .Where(a => a.Status == AppointmentStatus.Scheduled)
            .OrderBy(a => a.WhenUtc)
            .Take(10)
            .ToList();

        ViewBag.ActiveSessions = activeSessions;
        ViewBag.CompletedSessions = completedSessions;
        ViewBag.PendingViewings = pendingViewings;
        return View();
    }

    // Manager: Monitor specific session
    [HttpGet("tracking/manager/session/{sessionId}")]
    public IActionResult MonitorSession(int sessionId)
    {
        var session = _db.TrackingSessions
            .Include(s => s.Property)
            .Include(s => s.Agent)
            .Include(s => s.ViewingAppointment)
            .FirstOrDefault(s => s.Id == sessionId);

        if (session == null)
        {
            TempData["ErrorMessage"] = "Tracking session not found.";
            return RedirectToAction("ManagerDashboard");
        }

        ViewBag.Session = session;
        return View();
    }

    // Manager: Get session statistics
    [HttpGet("tracking/manager/stats")]
    public IActionResult GetSessionStats()
    {
        var today = DateTime.UtcNow.Date;

        var stats = new
        {
            ActiveSessions = _db.TrackingSessions.Count(s => s.Status == TrackingSessionStatus.Active),
            CompletedToday = _db.TrackingSessions.Count(s => s.Status == TrackingSessionStatus.Completed && s.EndedAt.HasValue && s.EndedAt.Value.Date == today),
            TotalSessions = _db.TrackingSessions.Count(),
            PendingViewings = _db.ViewingAppointments.Count(a => a.Status == AppointmentStatus.Scheduled)
        };

        return Json(new { success = true, stats });
    }

    // ═══════════════════════════════════════════════════════
    // API ENDPOINTS FOR REAL-TIME UPDATES
    // ═══════════════════════════════════════════════════════

    // API: Get session locations (for real-time updates)
    [HttpGet("tracking/api/session/{sessionId}/locations")]
    public IActionResult GetSessionLocations(int sessionId)
    {
        var session = _db.TrackingSessions
            .Include(s => s.Agent)
            .FirstOrDefault(s => s.Id == sessionId);

        if (session == null)
        {
            return Json(new { success = false, message = "Session not found" });
        }

        var timeSinceUpdate = session.LastUpdated.HasValue 
            ? (DateTime.UtcNow - session.LastUpdated.Value).TotalSeconds 
            : (double?)null;

        return Json(new
        {
            success = true,
            customerLat = session.CustomerLatitude,
            customerLng = session.CustomerLongitude,
            agentLat = session.AgentLatitude,
            agentLng = session.AgentLongitude,
            propertyLat = session.PropertyLatitude,
            propertyLng = session.PropertyLongitude,
            status = session.Status.ToString(),
            agentName = session.Agent?.Name ?? session.Agent?.DisplayName ?? "Agent",
            lastUpdated = session.LastUpdated,
            secondsSinceUpdate = timeSinceUpdate,
            isActive = session.Status == TrackingSessionStatus.Active
        });
    }
}

// Helper classes
public class LocationData
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class EndSessionData
{
    public string? Notes { get; set; }
}
