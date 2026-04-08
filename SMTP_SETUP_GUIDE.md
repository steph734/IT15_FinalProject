# Gmail SMTP Configuration Guide for EstateFlow

## Issue Fixed
The application was using **maileroo.com** which is no longer a reliable email service. The SMTP configuration has been updated to use **Gmail's SMTP server**.

## Steps to Configure Gmail SMTP

### 1. Enable 2-Factor Authentication (if not already enabled)
- Go to https://myaccount.google.com/security
- Scroll down to "2-Step Verification"
- Follow the steps to enable it (this is required for App Passwords)

### 2. Generate an App Password
- Go to https://myaccount.google.com/apppasswords
- Select "Mail" as the app
- Select "Windows Computer" (or your device type)
- Click "Generate"
- Copy the 16-character password that appears
- **Keep this password safe** - you'll need it for the configuration

### 3. Update appsettings.json
Replace the placeholder values with your Gmail credentials:

```json
"SmtpSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SmtpUsername": "your-email@gmail.com",
  "SmtpPassword": "your-16-character-app-password",
  "SenderEmail": "your-email@gmail.com",
  "SenderName": "EstateFlow - Real Estate Solutions"
}
```

### Example Configuration
```json
"SmtpSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SmtpUsername": "stephentate@gmail.com",
  "SmtpPassword": "abcd efgh ijkl mnop",
  "SenderEmail": "stephentate@gmail.com",
  "SenderName": "EstateFlow - Real Estate Solutions"
}
```

## Important Notes
- **Never commit credentials to Git** - use User Secrets for production environments
- For development: Use User Secrets Manager in Visual Studio
  - Right-click project → Manage User Secrets
  - Add your SMTP settings there
- Port 587 uses TLS (Transport Layer Security)
- The 16-character app password includes spaces - include them as-is or remove them (both work)

## Using User Secrets for Secure Development

### Set User Secrets in Visual Studio:
1. Right-click the RealEstate project
2. Select "Manage User Secrets"
3. Add your SMTP settings:

```json
{
  "SmtpSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-16-character-app-password",
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "EstateFlow - Real Estate Solutions"
  }
}
```

## Troubleshooting

### "Username and Password not accepted" error
- Verify you're using the 16-character **App Password**, not your regular Gmail password
- Ensure 2-Factor Authentication is enabled
- Check that spaces in the app password are handled correctly

### "Less secure app access" error
- This error appears when using a regular Gmail password
- You MUST use an App Password instead
- App Passwords are only available after enabling 2-Factor Authentication

### "SMTP Server connection timeout"
- Verify port 587 is correct (not 465 or 25)
- Check your firewall settings
- Verify internet connectivity

## Testing the Configuration

Once configured, restart the application and test by:
1. Navigate to https://localhost:7125/admin/login
2. Enter test credentials
3. If configuration is correct, you should receive an OTP email

## Alternative Email Services

If Gmail doesn't work for you, other reliable SMTP services:

### SendGrid
- SMTP Server: smtp.sendgrid.net
- Port: 587
- Username: "apikey"
- Password: Your SendGrid API key

### Resend
- SMTP Server: smtp.resend.com
- Port: 465
- Username: "resend"
- Password: Your Resend API key

### AWS SES
- SMTP Server: email-smtp.[region].amazonaws.com
- Port: 587
- Username & Password: From AWS SES dashboard

## Reference
- Gmail App Passwords: https://support.google.com/accounts/answer/185833
- Gmail SMTP Settings: https://support.google.com/mail/answer/7126229
