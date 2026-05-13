using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

Console.WriteLine("Testing Gmail SMTP with MailKit...");
Console.WriteLine("Server: smtp.gmail.com:587 (STARTTLS)");
Console.WriteLine("User:   stephentatel18@gmail.com");
Console.WriteLine();

try
{
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("EstateFlow Test", "stephentatel18@gmail.com"));
    message.To.Add(new MailboxAddress("Test", "stephentatel18@gmail.com"));
    message.Subject = "MailKit SMTP Test";
    message.Body = new TextPart("plain") { Text = "This is a test from MailKit." };

    using var client = new SmtpClient();

    Console.WriteLine("Connecting...");
    await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
    Console.WriteLine("Connected OK");

    client.AuthenticationMechanisms.Remove("XOAUTH2");

    Console.WriteLine("Authenticating...");
    await client.AuthenticateAsync("stephentatel18@gmail.com", "dvtg umcq mcmr upsz");
    Console.WriteLine("Authenticated OK");

    Console.WriteLine("Sending...");
    await client.SendAsync(message);
    Console.WriteLine("SENT SUCCESSFULLY!");

    await client.DisconnectAsync(true);
}
catch (MailKit.Security.AuthenticationException ex)
{
    Console.WriteLine($"AUTH FAILED: {ex.Message}");
    Console.WriteLine(">> The Gmail App Password is INVALID or EXPIRED.");
    Console.WriteLine(">> Go to: https://myaccount.google.com/apppasswords");
    Console.WriteLine(">> Delete the old App Password, create a new one, and update appsettings.json.");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.GetType().Name} - {ex.Message}");
    if (ex.InnerException != null)
        Console.WriteLine($"INNER: {ex.InnerException.Message}");
}
