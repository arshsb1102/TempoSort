using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using Npgsql;
using Dapper;

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
        var result = new Dictionary<string, object>();
        var isHealthy = true;

        // ✅ SMTP Check
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

            client.SendCompleted += (s, e) => { };
            client.ServicePoint.MaxIdleTime = 1;
            client.ServicePoint.ConnectionLimit = 1;

            client.SendMailAsync(new MailMessage
            {
                From = new MailAddress(username),
                To = { username },
                Subject = "HealthCheck",
                Body = "SMTP health check",
            }).Wait(1);

            result["smtp"] = "OK";
        }
        catch (Exception ex)
        {
            result["smtp"] = $"Failed: {ex.Message}";
            isHealthy = false;
        }

        // ✅ Database Check
        try
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            var test = conn.ExecuteScalar<int>("SELECT 1;");
            result["database"] = "OK";
        }
        catch (Exception ex)
        {
            result["database"] = $"Failed: {ex.Message}";
            isHealthy = false;
        }

        return isHealthy ? Ok(new { status = "Healthy", checks = result })
                         : StatusCode(503, new { status = "Unhealthy", checks = result });
    }
}
