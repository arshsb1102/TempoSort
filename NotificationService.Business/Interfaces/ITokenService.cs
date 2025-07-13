namespace NotificationService.Business.Interfaces;
public interface ITokenService
{
    string GenerateToken(Guid userId, string purpose, TimeSpan expiresIn);
    (bool IsValid, Guid? UserId) ValidateToken(string token, string expectedPurpose);
}
