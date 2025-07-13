using NotificationService.Business.Interfaces;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models.DBObjects;
using NotificationService.Models.DTOs;

namespace NotificationService.Business;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Guid> CreateTaskAsync(Guid userId, CreateTaskRequest request)
    {
        var task = new UserTask
        {
            TaskId = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            DueAt = request.DueAt,
            Priority = request.Priority,
            IsCompleted = false,
            CreatedOn = DateTime.UtcNow
        };

        await _taskRepository.CreateAsync(task);
        return task.TaskId;
    }

    public async Task<IEnumerable<TaskResponse>> GetTasksAsync(Guid userId)
    {
        var tasks = await _taskRepository.GetByUserIdAsync(userId);
        return tasks.Select(t => new TaskResponse
        {
            TaskId = t.TaskId,
            Title = t.Title,
            Description = t.Description,
            DueAt = t.DueAt,
            Priority = t.Priority,
            IsCompleted = t.IsCompleted,
            CreatedOn = t.CreatedOn,
            UpdatedOn = t.UpdatedOn
        });
    }

    public async Task<TaskResponse?> GetTaskByIdAsync(Guid userId, Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null || task.UserId != userId)
            return null;

        return new TaskResponse
        {
            TaskId = task.TaskId,
            Title = task.Title,
            Description = task.Description,
            DueAt = task.DueAt,
            Priority = task.Priority,
            IsCompleted = task.IsCompleted,
            CreatedOn = task.CreatedOn,
            UpdatedOn = task.UpdatedOn
        };
    }

    public async Task UpdateTaskAsync(Guid userId, Guid taskId, UpdateTaskRequest request)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null || task.UserId != userId)
            throw new UnauthorizedAccessException("Task not found or unauthorized");

        task.Title = request.Title;
        task.Description = request.Description;
        task.DueAt = request.DueAt;
        task.Priority = request.Priority;
        task.UpdatedOn = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);
    }

    public async Task DeleteTaskAsync(Guid userId, Guid taskId)
    {
        await _taskRepository.DeleteAsync(taskId, userId);
    }

    public async Task ToggleCompleteAsync(Guid userId, Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null || task.UserId != userId)
            throw new UnauthorizedAccessException("Task not found or unauthorized");

        task.IsCompleted = !task.IsCompleted;
        task.UpdatedOn = DateTime.UtcNow;
        await _taskRepository.UpdateAsync(task);
    }
}
