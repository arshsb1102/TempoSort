namespace NotificationService.DataAccess.Interfaces;

public interface ISmtpService
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
}