namespace NotificationService.Models.DTOs;

public class UserPreferencesDto
{
    public bool IsDigestEnabled { get; set; }
    public TimeSpan DigestTime { get; set; }
}