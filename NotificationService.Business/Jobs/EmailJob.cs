using Quartz;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace NotificationService.Business.Jobs;

public class EmailJob : IJob
{
    private readonly IConfiguration _config;

    public EmailJob(IConfiguration config)
    {
        _config = config;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var to = context.MergedJobDataMap.GetString("to");
        var subject = context.MergedJobDataMap.GetString("subject");
        var body = context.MergedJobDataMap.GetString("body");

        var smtp = _config.GetSection("SmtpSettings");

        using var client = new SmtpClient(smtp["Host"], int.TryParse(smtp["Port"], out var port) ? port : 587)
        {
            EnableSsl = bool.Parse(smtp["EnableSsl"] ?? "true"),
            Credentials = new NetworkCredential(smtp["Username"], smtp["Password"])
        };

        var message = new MailMessage
        {
            From = new MailAddress(smtp["FromEmail"] ?? "no-reply@example.com", smtp["FromName"] ?? "TempoSelf"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(to!);
        await client.SendMailAsync(message);
    }
}

