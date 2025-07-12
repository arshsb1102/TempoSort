using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using NotificationService.Business.Interfaces;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtp = _config.GetSection("SmtpSettings");

        var host = smtp["Host"] ?? throw new Exception("SMTP Host is not configured");
        var port = int.Parse(smtp["Port"] ?? throw new Exception("SMTP Port is not configured"));
        var username = smtp["Username"] ?? throw new Exception("SMTP Username is not configured");
        var password = smtp["Password"] ?? throw new Exception("SMTP Password is not configured");
        var fromEmail = smtp["FromEmail"] ?? throw new Exception("SMTP FromEmail is not configured");
        var fromName = smtp["FromName"] ?? "Notification Service";
        var enableSsl = bool.Parse(smtp["EnableSsl"] ?? "true");

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl
        };

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(to);

        await client.SendMailAsync(message);
    }
}
