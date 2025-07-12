using NotificationService.Business.Interfaces;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models;

namespace NotificationService.Business;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task SignUpAsync(User user)
    {
        var existing = await _userRepository.GetByEmailAsync(user.Email);
        if (existing is not null)
            throw new Exception("User already exists");

        user.CreatedAt = DateTime.UtcNow;
        await _userRepository.CreateAsync(user);
    }
}
