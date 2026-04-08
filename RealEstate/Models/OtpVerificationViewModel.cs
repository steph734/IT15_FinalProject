namespace RealEstate.Models;

public class OtpVerificationViewModel
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
    public int ExpirySeconds { get; set; } = 600; // 10 minutes default
}
