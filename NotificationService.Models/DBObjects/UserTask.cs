namespace NotificationService.Models.DBObjects;

public class UserTask
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public DateTime? DueAt { get; set; }
    public int Priority { get; set; } = 2;
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public Guid? TeamId { get; set; } 
}
