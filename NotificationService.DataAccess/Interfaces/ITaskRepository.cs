using NotificationService.Models.DBObjects;

namespace NotificationService.DataAccess.Interfaces;

public interface ITaskRepository
{
    Task CreateAsync(UserTask task);
    Task<UserTask?> GetByIdAsync(Guid taskId);
    Task<IEnumerable<UserTask>> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(UserTask task);
    Task DeleteAsync(Guid taskId, Guid userId);
}
