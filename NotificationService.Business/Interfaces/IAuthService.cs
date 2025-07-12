using NotificationService.Models;

namespace NotificationService.Business.Interfaces;

public interface IAuthService
{
    Task SignUpAsync(User user);
    Task<string> LoginAsync(string email, string password);
}
