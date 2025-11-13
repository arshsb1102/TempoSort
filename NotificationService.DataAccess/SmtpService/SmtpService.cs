using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models.SMTP;
using Resend;
using Microsoft.Extensions.Configuration;

namespace NotificationService.DataAccess.SmtpService;

public class SmtpService(IOptions<SmtpSettings> options, IConfiguration _config) : ISmtpService
{
    private readonly SmtpSettings _settings = options.Value;
    private readonly IConfiguration config = _config;
    private readonly bool isHttp = _config.GetValue<bool>("UseResendHttpClient");
    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        if (isHttp)
        {
            var apiKey = config["Resend:Api_Key"] ?? throw new Exception("Resend API Key is not configured");
            IResend resend = ResendClient.Create(apiKey);
            var resp = await resend.EmailSendAsync(new EmailMessage()
            {
                From = $"{_settings.FromName} <{_settings.FromEmail}>",
                To = toEmail,
                Subject = subject,
                HtmlBody = htmlBody
            });
        }
        else
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}