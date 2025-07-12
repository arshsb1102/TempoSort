using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IConfiguration _config;

    public HealthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public IActionResult CheckHealth()
    {
        // Check SMTP
        try
        {
            var smtp = _config.GetSection("SmtpSettings");
            var host = smtp["Host"];
            var port = int.Parse(smtp["Port"] ?? "587");
            var username = smtp["Username"] ?? "";
            var password = smtp["Password"] ?? "";
            var enableSsl = bool.Parse(smtp["EnableSsl"] ?? "true");

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(username, password)
            };

            // SMTP test: try to connect
            client.SendCompleted += (s, e) => { }; // prevent event warnings
            client.ServicePoint.MaxIdleTime = 1;  // tiny timeout
            client.ServicePoint.ConnectionLimit = 1;

            client.SendMailAsync(new MailMessage
            {
                From = new MailAddress(username),
                To = { username }, // Dummy loopback
                Subject = "HealthCheck",
                Body = "SMTP health check",
            }).Wait(1); // trigger async connect — will timeout immediately

            return Ok(new { status = "Healthy", smtp = "OK" });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { status = "Unhealthy", smtp = "Failed", error = ex.Message });
        }
    }
}
