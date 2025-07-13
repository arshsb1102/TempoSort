using NotificationService.Models.DTOs;

namespace NotificationService.Business.Interfaces;

public interface ITaskService
{
    Task<Guid> CreateTaskAsync(Guid userId, CreateTaskRequest request);
    Task<IEnumerable<TaskResponse>> GetTasksAsync(Guid userId);
    Task<TaskResponse?> GetTaskByIdAsync(Guid userId, Guid taskId);
    Task UpdateTaskAsync(Guid userId, Guid taskId, UpdateTaskRequest request);
    Task DeleteTaskAsync(Guid userId, Guid taskId);
    Task ToggleCompleteAsync(Guid userId, Guid taskId);
}
