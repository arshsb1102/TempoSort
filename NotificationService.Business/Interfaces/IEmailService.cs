using NotificationService.Models.DBObjects;

namespace NotificationService.Business.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendVerificationEmail(User user, string token);
    Task SendWelcomeEmail(string userEmail, string userName);
    Task SendDailyDigest(string email, string name, Guid userId);
}