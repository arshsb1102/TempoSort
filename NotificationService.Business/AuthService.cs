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

public class AuthService(IUserRepository userRepository, IConfiguration config) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _config = config;
    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!user.IsEmailVerified)
            throw new InvalidOperationException("Please verify your email first.");

        if (user.IsEmailDead)
            throw new InvalidOperationException("Email is unreachable. Please use a valid email.");

        return GenerateJwtToken(user);
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

        // TODO: Trigger verification email
    }

}