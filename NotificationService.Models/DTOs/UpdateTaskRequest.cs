namespace NotificationService.Models.DTOs;

public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueAt { get; set; }
    public int Priority { get; set; } = 2;
}
