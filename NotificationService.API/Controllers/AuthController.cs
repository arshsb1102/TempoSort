using Microsoft.AspNetCore.Mvc;
using NotificationService.Business.Interfaces;
using NotificationService.Models;
using NotificationService.Models.DBObjects;
using NotificationService.Models.Request;

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

    [HttpPost("signup-old")]
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
            var bearerFormat = "Bearer " + Convert.ToString(token);
            return Ok(new { token, bearerFormat });
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] RegisterRequest request)
    {
        await _authService.RegisterUserAsync(request);
        return Ok(new { message = "Verification email sent." });
    }
}
