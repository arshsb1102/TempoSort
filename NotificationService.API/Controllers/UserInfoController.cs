using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models.DTOs;
using System.Security.Claims;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/user")]
public class UserInfoController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    public UserInfoController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(new { userId, name, email });
    }

    [Authorize]
    [HttpGet("getInfo")]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email != null)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return Ok(user);
        }
        return Ok();
    }
    [Authorize]
    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences(UserPreferencesDto userPreferences)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null)
        {
            await _userRepository.UpdateDigestSettingsAsync(Guid.Parse(userId), userPreferences);
            return Ok();
        }
        return Ok();
    }
}
