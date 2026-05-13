using RealEstate.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services;

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

        _context.ViewingAppointments.Add(new RealEstate.Models.ViewingAppointment
        {
            PropertyId = 1,
            CustomerName = "Juan Dela Cruz",
            CustomerPhone = "09171234567",
            CustomerPhotoUrl = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?auto=format&fit=crop&w=200&q=80",
            WhenUtc = DateTime.UtcNow.AddDays(1).AddHours(10),
            Status = AppointmentStatus.Scheduled
        });

        _context.ViewingAppointments.Add(new RealEstate.Models.ViewingAppointment
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

    public IReadOnlyList<RealEstate.Models.ViewingAppointment> GetAll() 
        => _context.ViewingAppointments.OrderBy(a => a.WhenUtc).ToList().AsReadOnly();

    public IReadOnlyList<RealEstate.Models.ViewingAppointment> GetUpcoming(int days = 7) 
        => _context.ViewingAppointments
            .Where(a => a.WhenUtc >= DateTime.UtcNow && a.WhenUtc <= DateTime.UtcNow.AddDays(days))
            .OrderBy(a => a.WhenUtc)
            .ToList()
            .AsReadOnly();

    public RealEstate.Models.ViewingAppointment? Get(int id) 
        => _context.ViewingAppointments.FirstOrDefault(a => a.Id == id);

    public RealEstate.Models.ViewingAppointment Add(RealEstate.Models.ViewingAppointment appt)
    {
        _context.ViewingAppointments.Add(appt);
        _context.SaveChanges();
        return appt;
    }

    public void Update(RealEstate.Models.ViewingAppointment appt)
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

    public IReadOnlyList<RealEstate.Models.ViewingAppointment> GetByProperty(int propertyId)
        => _context.ViewingAppointments
            .Where(a => a.PropertyId == propertyId)
            .OrderBy(a => a.WhenUtc)
            .ToList()
            .AsReadOnly();

    public IReadOnlyList<RealEstate.Models.ViewingAppointment> GetByStatus(AppointmentStatus status)
        => _context.ViewingAppointments
            .Where(a => a.Status == status)
            .OrderBy(a => a.WhenUtc)
            .ToList()
            .AsReadOnly();
}
