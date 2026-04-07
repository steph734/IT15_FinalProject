namespace RealEstate.Models;

public class OtpVerification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OtpCode { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public User? User { get; set; }
}
