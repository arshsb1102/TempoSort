using NotificationService.Models.DBObjects;
using NotificationService.Models.Request;

namespace NotificationService.Business.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(string email, string password);
    Task RegisterUserAsync(RegisterRequest request);
    Task MarkEmailVerified(string token);
    Task ResendVerificationEmailAsync(string email);
    Task DeleteUser(string email, string password);
}
