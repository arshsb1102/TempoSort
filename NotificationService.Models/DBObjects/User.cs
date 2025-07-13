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
}