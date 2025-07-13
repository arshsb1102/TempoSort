using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using NotificationService.Business.Interfaces;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models.DBObjects;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly EmailTemplateRenderer _templateRenderer;
    private readonly ISmtpService _smtp;

    public EmailService(IConfiguration config, ISmtpService smtpService, EmailTemplateRenderer emailTemplateRenderer)
    {
        _config = config;
        _smtp = smtpService;
        _templateRenderer = emailTemplateRenderer;
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
    public async Task SendVerificationEmail(User user, string token)
    {
        var link = $"https://yourfrontend.com/verify-email?token={token}";
        var html = _templateRenderer.RenderTemplate("Templates/Email/VerifyEmailTemplate.html", new
        {
            name = user.Name,
            link = link,
            show_button = true,
            expiry = DateTime.UtcNow.AddDays(1).ToString("f")
        });

        await _smtp.SendAsync(user.Email, "Verify your TempoSort email", html);
    }
}
