using System.Net;
using System.Net.Mail;

namespace RealEstate.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendOtpEmailAsync(string recipientEmail, string otpCode, string userName)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var smtpServer = smtpSettings["SmtpServer"];
            var smtpPort = int.Parse(smtpSettings["SmtpPort"] ?? "587");
            var smtpUsername = smtpSettings["SmtpUsername"];
            var smtpPassword = smtpSettings["SmtpPassword"];

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                var subject = "Your OTP Verification Code";
                var body = GenerateOtpEmailBody(otpCode, userName);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUsername ?? "noreply@realestate.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation($"OTP email sent successfully to {recipientEmail}");
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send OTP email to {recipientEmail}: {ex.Message}");
            return false;
        }
    }

    private string GenerateOtpEmailBody(string otpCode, string userName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .otp-box {{ background-color: #fff; border: 2px solid #4CAF50; padding: 20px; text-align: center; margin: 20px 0; border-radius: 5px; }}
        .otp-code {{ font-size: 32px; font-weight: bold; color: #4CAF50; letter-spacing: 5px; }}
        .footer {{ background-color: #f0f0f0; padding: 15px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 5px 5px; }}
        .warning {{ color: #d9534f; font-weight: bold; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>OTP Verification</h2>
        </div>
        <div class=""content"">
            <p>Hello {userName},</p>
            <p>You have requested to log in to your account. Please use the following 6-digit OTP code to verify your identity:</p>
            
            <div class=""otp-box"">
                <div class=""otp-code"">{otpCode}</div>
            </div>
            
            <p><strong>Important:</strong></p>
            <ul>
                <li>This OTP code will expire in 10 minutes.</li>
                <li class=""warning"">Never share this code with anyone.</li>
                <li>If you did not request this login, please ignore this email.</li>
            </ul>
            
            <p>Thank you,<br/>Real Estate Platform Team</p>
        </div>
        <div class=""footer"">
            <p>&copy; 2025 Real Estate Platform. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }
}
