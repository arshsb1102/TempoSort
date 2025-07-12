using NotificationService.Models;

namespace NotificationService.DataAccess.Interfaces;
public interface INotificationRepository
{
    Task CreateAsync(Notification notification);
    Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId);
    Task MarkAsReadAsync(int notificationId, int userId);
}
