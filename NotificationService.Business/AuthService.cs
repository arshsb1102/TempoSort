using NotificationService.Business.Interfaces;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using NotificationService.Models.Request;
using NotificationService.Models.DBObjects;


namespace NotificationService.Business;

public class AuthService(IUserRepository userRepository, IConfiguration config, ITokenService tokenService, IEmailService emailService, IWelcomeEmailScheduler welcomeEmailScheduler) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _config = config;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IEmailService _emailService = emailService;
    private readonly IWelcomeEmailScheduler _welcomeEmailScheduler = welcomeEmailScheduler;
    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!user.IsEmailVerified)
            throw new InvalidOperationException("Please verify your email first.");

        if (user.IsEmailDead)
            throw new InvalidOperationException("Email is unreachable. Please use a valid email.");

        var token = GenerateJwtToken(user);
        if (!user.IsWelcomeDone)
        {
            await _welcomeEmailScheduler.ScheduleAsync(user, delayInMinutes: 1);
        }
        return token;
    }
    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpiresInMinutes"]!)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task RegisterUserAsync(RegisterRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new Exception("Email already registered.");

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email.ToLower().Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedOn = DateTime.UtcNow,
            IsEmailVerified = false,
            IsEmailDead = false
        };

        await _userRepository.CreateUserAsync(user);
        await ResendVerificationEmailAsync(user.Email);
    }
    public async Task ResendVerificationEmailAsync(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower());
            if (user is null)
                throw new Exception("Email not registered.");

            if (user.IsEmailVerified)
                throw new Exception("Email is already verified.");
            if (user.LastVerificationSentOn.HasValue &&
                (DateTime.UtcNow - user.LastVerificationSentOn.Value).TotalMinutes < 2)
            {
                throw new Exception("Please wait before resending verification email.");
            }
            var token = _tokenService.GenerateToken(user.UserId, "email-verification", TimeSpan.FromDays(1));
            await _emailService.SendVerificationEmail(user, token);
            await _userRepository.UpdateLastVerificationSentOnAsync(user.UserId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to resend verification email: {ex.Message}");
        }
    }
    public async Task MarkEmailVerified(string token)
    {
        var (IsValid, UserId) = _tokenService.ValidateToken(token, "email-verification");
        if (!IsValid)
            throw new Exception("Invalid or expired verification link.");

        var userId = UserId!.Value;
        await _userRepository.MarkEmailVerified(userId);
    }
    public async Task DeleteUser(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower()) ?? throw new Exception("Email not registered.");
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new UnauthorizedAccessException("Invalid password");
        await _userRepository.DeleteUser(user.UserId);
    }
}