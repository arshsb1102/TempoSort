using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Business.Interfaces;
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
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        await _authService.MarkEmailVerified(token);
        return Redirect("https://tempo-sort.vercel.app/login");
    }
    [HttpGet("EmailVerified")]
    public IActionResult EmailVerified()
    {
        return Ok("Email Verified Successfully");
    }
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(string Email)
    {
        await _authService.ResendVerificationEmailAsync(Email);
        return Ok(new { message = "Verification email resent." });
    }
    [Authorize]
    [HttpPost("delete-user")]
    public async Task<IActionResult> DeleteUser([FromBody] LoginRequest request)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? throw new Exception("Data is Null");
        await _authService.DeleteUser(email, request.Password);
        return Ok(new { message = "Account has been Deleted" });
    }
}
