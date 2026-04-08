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
            var senderEmail = smtpSettings["SenderEmail"];
            var senderName = smtpSettings["SenderName"] ?? "RealEstate Team";

            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
            {
                _logger.LogError("SMTP settings are not configured properly");
                return false;
            }

            if (string.IsNullOrEmpty(senderEmail))
            {
                _logger.LogError("Sender email is not configured. Please add 'SenderEmail' to SmtpSettings in appsettings.json");
                return false;
            }

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.Timeout = 10000; // 10 second timeout

                var subject = "Your OTP Verification Code";
                var body = GenerateOtpEmailBody(otpCode, userName);

                using (var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    mailMessage.To.Add(recipientEmail);

                    try
                    {
                        await client.SendMailAsync(mailMessage);
                        _logger.LogInformation($"OTP email sent successfully to {recipientEmail} from {senderEmail}");
                        return true;
                    }
                    catch (SmtpException smtpEx)
                    {
                        _logger.LogError($"SMTP Error sending OTP to {recipientEmail}: StatusCode={smtpEx.StatusCode}, Message={smtpEx.Message}");
                        return false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send OTP email to {recipientEmail}: {ex.GetType().Name} - {ex.Message}");
            return false;
        }
    }

    private string GenerateOtpEmailBody(string otpCode, string userName)
    {
        return $$"""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>OTP Verification</title>
    <style type="text/css">
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }
        .container {
            max-width: 600px;
            margin: 20px auto;
            background-color: white;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        .header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px 20px;
            text-align: center;
        }
        .header h1 {
            margin: 0;
            font-size: 28px;
        }
        .header p {
            margin: 5px 0 0 0;
            font-size: 14px;
            opacity: 0.9;
        }
        .content {
            padding: 30px 20px;
        }
        .greeting {
            font-size: 16px;
            margin-bottom: 20px;
        }
        .otp-box {
            background-color: #f9f9f9;
            border: 2px dashed #667eea;
            padding: 25px;
            text-align: center;
            margin: 25px 0;
            border-radius: 8px;
        }
        .otp-code {
            font-size: 48px;
            font-weight: bold;
            color: #667eea;
            letter-spacing: 8px;
            font-family: 'Courier New', monospace;
            margin: 0;
        }
        .otp-label {
            font-size: 12px;
            color: #999;
            margin-top: 10px;
        }
        .instructions {
            background-color: #f0f7ff;
            border-left: 4px solid #667eea;
            padding: 15px;
            margin: 20px 0;
            border-radius: 4px;
        }
        .instructions h3 {
            margin: 0 0 10px 0;
            color: #667eea;
            font-size: 14px;
        }
        .instructions ul {
            margin: 0;
            padding-left: 20px;
        }
        .instructions li {
            margin: 8px 0;
            font-size: 14px;
            color: #555;
        }
        .warning {
            color: #d9534f;
            font-weight: bold;
        }
        .footer {
            background-color: #f5f5f5;
            padding: 20px;
            text-align: center;
            font-size: 12px;
            color: #888;
            border-top: 1px solid #eee;
        }
        .footer p {
            margin: 5px 0;
        }
        .divider {
            height: 1px;
            background-color: #eee;
            margin: 20px 0;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>Email Verification</h1>
            <p>RealEstate Platform</p>
        </div>

        <div class="content">
            <div class="greeting">
                <p>Hello <strong>{{userName}}</strong>,</p>
            </div>

            <p>You have requested to log in to your RealEstate account. Please use the following One-Time Password (OTP) to verify your identity:</p>

            <div class="otp-box">
                <div class="otp-code">{{otpCode}}</div>
                <div class="otp-label">Your One-Time Password</div>
            </div>

            <div class="instructions">
                <h3>⏱️ Important Information:</h3>
                <ul>
                    <li>This OTP code will expire in <strong>10 minutes</strong></li>
                    <li>Each code can only be used once</li>
                    <li><span class="warning">Never share this code with anyone</span></li>
                    <li>If you did not request this login, please ignore this email and secure your account</li>
                </ul>
            </div>

            <div class="divider"></div>

            <p>If you have any questions or need assistance, please contact our support team.</p>

            <p>Best regards,<br><strong>RealEstate Platform Team</strong></p>
        </div>

        <div class="footer">
            <p>&copy; 2025 RealEstate Platform. All rights reserved.</p>
            <p>This is an automated email. Please do not reply to this message.</p>
        </div>
    </div>
</body>
</html>
""";
    }
}
