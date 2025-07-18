namespace NotificationService.Models.DBObjects;

public class User
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime? VerifiedOn { get; set; }
    public bool IsEmailDead { get; set; }
    public DateTime? LastVerificationSentOn { get; set; }
    public TimeSpan? DigestTime { get; set; } = new TimeSpan(8, 0, 0); // Default 08:00
    public string? Theme { get; set; } = "light";
    public bool IsDigestEnabled { get; set; } = true;
    public bool? IsWelcomeDone { get; set; }
    public TimeSpan ModifiedOn { get; set; }
}