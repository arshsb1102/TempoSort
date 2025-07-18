using Microsoft.AspNetCore.Mvc;
using NotificationService.Business.Interfaces;

[ApiController]
[Route("api/test-email")]
public class EmailTestController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailTestController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("digest")]
    public async Task<IActionResult> SendTestDigest([FromBody] TestDigestRequest request)
    {
        try
        {
            await _emailService.SendDailyDigest(request.Email, request.Name, request.UserId);
            return Ok("Digest email sent successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error sending email: {ex.Message}");
        }
    }
}

public class TestDigestRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
