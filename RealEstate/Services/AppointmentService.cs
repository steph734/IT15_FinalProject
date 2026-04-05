using RealEstate.Models;

namespace RealEstate.Services;

public enum AppointmentStatus { Scheduled, Confirmed, Completed, Cancelled }

public class ViewingAppointment
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerPhotoUrl { get; set; } = string.Empty;
    public DateTime WhenUtc { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
}

public class AppointmentService
{
    private readonly ApplicationDBContext _context;

    public AppointmentService(ApplicationDBContext context)
    {
        _context = context;
        SeedDemoDataIfEmpty();
    }

    private void SeedDemoDataIfEmpty()
    {
        if (_context.ViewingAppointments.Any())
            return;

        _context.ViewingAppointments.Add(new ViewingAppointment
        {
            PropertyId = 1,
            CustomerName = "Juan Dela Cruz",
            CustomerPhone = "09171234567",
            CustomerPhotoUrl = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?auto=format&fit=crop&w=200&q=80",
            WhenUtc = DateTime.UtcNow.AddDays(1).AddHours(10),
            Status = AppointmentStatus.Scheduled
        });

        _context.ViewingAppointments.Add(new ViewingAppointment
        {
            PropertyId = 2,
            CustomerName = "Maria Santos",
            CustomerPhone = "09179876543",
            CustomerPhotoUrl = "https://images.unsplash.com/photo-1545996124-1a9d7f1a4b68?auto=format&fit=crop&w=200&q=80",
            WhenUtc = DateTime.UtcNow.AddDays(2).AddHours(14),
            Status = AppointmentStatus.Scheduled
        });

        _context.SaveChanges();
    }

    public IReadOnlyList<ViewingAppointment> GetAll() 
        => _context.ViewingAppointments.OrderBy(a => a.WhenUtc).ToList().AsReadOnly();

    public IReadOnlyList<ViewingAppointment> GetUpcoming(int days = 7) 
        => _context.ViewingAppointments
            .Where(a => a.WhenUtc >= DateTime.UtcNow && a.WhenUtc <= DateTime.UtcNow.AddDays(days))
            .OrderBy(a => a.WhenUtc)
            .ToList()
            .AsReadOnly();

    public ViewingAppointment? Get(int id) 
        => _context.ViewingAppointments.FirstOrDefault(a => a.Id == id);

    public ViewingAppointment Add(ViewingAppointment appt)
    {
        _context.ViewingAppointments.Add(appt);
        _context.SaveChanges();
        return appt;
    }

    public void Update(ViewingAppointment appt)
    {
        _context.ViewingAppointments.Update(appt);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var appt = _context.ViewingAppointments.FirstOrDefault(a => a.Id == id);
        if (appt != null)
        {
            _context.ViewingAppointments.Remove(appt);
            _context.SaveChanges();
        }
    }

    public IReadOnlyList<ViewingAppointment> GetByProperty(int propertyId)
        => _context.ViewingAppointments
            .Where(a => a.PropertyId == propertyId)
            .OrderBy(a => a.WhenUtc)
            .ToList()
            .AsReadOnly();

    public IReadOnlyList<ViewingAppointment> GetByStatus(AppointmentStatus status)
        => _context.ViewingAppointments
            .Where(a => a.Status == status)
            .OrderBy(a => a.WhenUtc)
            .ToList()
            .AsReadOnly();
}
