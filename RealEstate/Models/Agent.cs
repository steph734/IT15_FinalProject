using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models;

public class Agent
{
    public int Id { get; set; }

    [ForeignKey("User")]
    public int? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    // Identity
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }

    // Contact
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    // Professional
    public int YearsOfExperience { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;

    // Documents (stored as paths/URLs)
    public string ResumeFilePath { get; set; } = string.Empty;
    public string ValidIdFilePath { get; set; } = string.Empty;
    public string? ProfilePhotoPath { get; set; }

    public bool IsArchived { get; set; } = false;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Legacy fields used elsewhere in the app (kept for compatibility)
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;

    // Preferred display name
    public string DisplayName => string.Join(" ", new[] { FirstName, MiddleName, LastName }.Where(s => !string.IsNullOrWhiteSpace(s)));
}
