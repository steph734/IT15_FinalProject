namespace RealEstate.Models;

public class TrackingSession
{
    public int Id { get; set; }
    public int ViewingAppointmentId { get; set; }
    public int PropertyId { get; set; }
    public int? AgentId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public double CustomerLatitude { get; set; }
    public double CustomerLongitude { get; set; }
    public double AgentLatitude { get; set; }
    public double AgentLongitude { get; set; }
    public double PropertyLatitude { get; set; }
    public double PropertyLongitude { get; set; }
    public TrackingSessionStatus Status { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
    public string? Notes { get; set; }
    
    // Navigation Properties
    public ViewingAppointment? ViewingAppointment { get; set; }
    public Property? Property { get; set; }
    public Agent? Agent { get; set; }
}

public enum TrackingSessionStatus
{
    Pending,
    Active,
    Completed,
    Cancelled
}
