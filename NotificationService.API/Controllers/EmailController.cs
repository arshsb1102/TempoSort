using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Business.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    [Authorize]
    public async Task<IActionResult> SendEmailToLoggedInUser()
    {
        // Get user's email from JWT claims
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userEmail))
            return BadRequest("Email not found in JWT token.");

        var subject = "Hello from Notification Service!";
        var body = $"Hi,\n\nThis is a plain text email sent to your account ({userEmail}) via our notification system.";

        await _emailService.SendEmailAsync(userEmail, subject, body);

        return Ok(new { message = "Email sent successfully." });
    }
}
