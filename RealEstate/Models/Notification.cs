using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models;

/// <summary>
/// Notification system for alerting users about important events
/// </summary>
public class Notification
{
    public int NotificationId { get; set; }

    /// <summary>User who receives this notification</summary>
    public int? UserId { get; set; }

    /// <summary>Details of the user</summary>
    public ApplicationUser User { get; set; }

    /// <summary>Employee associated with the notification</summary>
    public int? EmployeeId { get; set; }

    /// <summary>Details of the employee</summary>
    public Employee? Employee { get; set; }

    /// <summary>Agent associated with the notification</summary>
    public int? AgentId { get; set; }

    /// <summary>Details of the agent</summary>
    public Agent? Agent { get; set; }

    /// <summary>Appointment related to the notification</summary>
    public int? AppointmentId { get; set; }

    /// <summary>Details of the appointment</summary>
    public Appointment? Appointment { get; set; }

    /// <summary>Title of the notification</summary>
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>Detailed message</summary>
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    // Additional properties used by controllers
    public string NotificationType { get; set; } = string.Empty;
    public int? RelatedEntityId { get; set; }
    public string RelatedEntityType { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string DataJson { get; set; } = string.Empty;
}
