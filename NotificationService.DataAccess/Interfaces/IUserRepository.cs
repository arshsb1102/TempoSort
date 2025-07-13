using NotificationService.Models;
using NotificationService.Models.DBObjects;

namespace NotificationService.DataAccess.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task CreateUserAsync(User user);
}
