using NotificationService.Models;

namespace NotificationService.Business.Interfaces;
public interface INotificationHelper
{
    Task CreateAsync(int userId, string message, string? link, string type);
    Task<IEnumerable<Notification>> GetAllAsync(int userId);
    Task<IEnumerable<Notification>> GetUnreadAsync(int userId);
    Task MarkAsReadAsync(int notificationId, int userId);
}
