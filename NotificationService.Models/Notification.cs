namespace NotificationService.Models;
public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; } = null!;
    public string? Link { get; set; }
    public string Type { get; set; } = "info"; // info, warning, success, error
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
