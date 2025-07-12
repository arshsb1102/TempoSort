using NotificationService.Business.Interfaces;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models;

namespace NotificationService.Business;

public class NotificationHelper : INotificationHelper
{
    private readonly INotificationRepository _repo;

    public NotificationHelper(INotificationRepository repo)
    {
        _repo = repo;
    }

    public async Task CreateAsync(int userId, string message, string? link, string type)
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            Link = link,
            Type = type
        };
        await _repo.CreateAsync(notification);
    }

    public Task<IEnumerable<Notification>> GetAllAsync(int userId) => _repo.GetByUserIdAsync(userId);
    public Task<IEnumerable<Notification>> GetUnreadAsync(int userId) => _repo.GetUnreadByUserIdAsync(userId);
    public Task MarkAsReadAsync(int notificationId, int userId) => _repo.MarkAsReadAsync(notificationId, userId);
}
