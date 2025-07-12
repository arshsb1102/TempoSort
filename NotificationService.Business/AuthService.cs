using NotificationService.Business.Interfaces;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models;
using BCrypt.Net;

namespace NotificationService.Business;

public class AuthService(IUserRepository userRepository) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task SignUpAsync(User user)
    {
        var existing = await _userRepository.GetByEmailAsync(user.Email);
        if (existing is not null)
            throw new Exception("User already exists");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.CreatedAt = DateTime.UtcNow;
        await _userRepository.CreateAsync(user);
    }
}
