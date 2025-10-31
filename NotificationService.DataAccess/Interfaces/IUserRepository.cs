using NotificationService.Models;
using NotificationService.Models.DBObjects;
using NotificationService.Models.DTOs;

namespace NotificationService.DataAccess.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task CreateUserAsync(User user);
    Task MarkEmailVerified(Guid userId);
    Task UpdateLastVerificationSentOnAsync(Guid userId);
    Task UpdateWelcomeOnAsync(Guid userid);
    Task UpdateDigestSettingsAsync(Guid userid, UserPreferencesDto preferencesDto);
    Task<IEnumerable<User>> GetUsersForDigestAsync(int hour, int minute);
    Task<bool> DeleteUser(Guid userId);
    Task<bool> SwitchUserDigest(Guid userId);
}
