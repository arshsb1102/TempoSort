namespace NotificationService.Models;
public class NotificationCreateDto
{
    public string Message { get; set; } = null!;
    public string? Link { get; set; }
    public string Type { get; set; } = "info"; // or warning, error, success
}
