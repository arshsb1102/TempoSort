using Microsoft.AspNetCore.Mvc;
using NotificationService.Business.Interfaces;
using NotificationService.Models;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] User user)
    {
        try
        {
            await _authService.SignUpAsync(user);
            return Ok("Signup successful");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
