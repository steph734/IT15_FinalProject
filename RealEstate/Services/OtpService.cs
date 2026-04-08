using RealEstate.Models;
using Microsoft.EntityFrameworkCore;

namespace RealEstate.Services;

public class OtpService
{
    private readonly ApplicationDBContext _context;
    private readonly ILogger<OtpService> _logger;
    private const int OTP_LENGTH = 6;
    private const int OTP_EXPIRY_MINUTES = 10;

    public OtpService(ApplicationDBContext context, ILogger<OtpService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Generates a new OTP for a user and saves it to the database
    /// </summary>
    public async Task<string> GenerateOtpAsync(int userId)
    {
        try
        {
            // Invalidate any existing non-expired OTPs for this user
            var existingOtps = await _context.OtpVerifications
                .Where(o => o.UserId == userId && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var otp in existingOtps)
            {
                otp.IsUsed = true;
            }

            // Generate a new 6-digit OTP
            var random = new Random();
            var otpCode = random.Next(100000, 999999).ToString();

            var otpVerification = new OtpVerification
            {
                UserId = userId,
                OtpCode = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(OTP_EXPIRY_MINUTES),
                IsUsed = false
            };

            _context.OtpVerifications.Add(otpVerification);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"OTP generated for UserId: {userId}, ExpiresAt: {otpVerification.ExpiresAt}, ExpiryMinutes: {OTP_EXPIRY_MINUTES}");
            return otpCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating OTP for UserId {userId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Verifies if the provided OTP is valid for the user
    /// </summary>
    public async Task<bool> VerifyOtpAsync(int userId, string otpCode)
    {
        try
        {
            var otp = await _context.OtpVerifications
                .FirstOrDefaultAsync(o => 
                    o.UserId == userId && 
                    o.OtpCode == otpCode && 
                    !o.IsUsed && 
                    o.ExpiresAt > DateTime.UtcNow);

            if (otp == null)
            {
                _logger.LogWarning($"Invalid or expired OTP provided for UserId: {userId}");
                return false;
            }

            // Mark OTP as used
            otp.IsUsed = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"OTP verified successfully for UserId: {userId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error verifying OTP for UserId {userId}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets the remaining time in seconds for OTP expiry
    /// </summary>
    public async Task<int> GetOtpExpirySecondsAsync(int userId)
    {
        try
        {
            var otp = await _context.OtpVerifications
                .Where(o => o.UserId == userId && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null)
            {
                _logger.LogWarning($"No valid OTP found for UserId {userId}");
                return 0;
            }

            var remainingSeconds = (int)(otp.ExpiresAt - DateTime.UtcNow).TotalSeconds;
            _logger.LogInformation($"OTP expiry check for UserId: {userId}, CreatedAt: {otp.CreatedAt}, ExpiresAt: {otp.ExpiresAt}, RemainingSeconds: {remainingSeconds}");
            return Math.Max(0, remainingSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting OTP expiry for UserId {userId}: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Cleans up expired OTPs from the database
    /// </summary>
    public async Task CleanupExpiredOtpsAsync()
    {
        try
        {
            var expiredOtps = await _context.OtpVerifications
                .Where(o => o.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            if (expiredOtps.Count > 0)
            {
                _context.OtpVerifications.RemoveRange(expiredOtps);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Cleaned up {expiredOtps.Count} expired OTPs");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cleaning up expired OTPs: {ex.Message}");
        }
    }
}
