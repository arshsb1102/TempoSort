using NotificationService.Business.Interfaces;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;


namespace NotificationService.Business;

public class AuthService(IUserRepository userRepository,IConfiguration config) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _config = config;
    public async Task SignUpAsync(User user)
    {
        var existing = await _userRepository.GetByEmailAsync(user.Email);
        if (existing is not null)
            throw new Exception("User already exists");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.CreatedAt = DateTime.UtcNow;
        await _userRepository.CreateAsync(user);
    }
     public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new Exception("Invalid email or password");

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
}