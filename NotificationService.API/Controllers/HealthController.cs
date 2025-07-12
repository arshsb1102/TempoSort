using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly IConfiguration _config;

    public HealthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public IActionResult HealthCheck()
    {
        var connString = _config.GetConnectionString("DefaultConnection");

        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            return Ok(new { status = "Healthy", database = "Connected" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "Unhealthy", database = "Disconnected", error = ex.Message });
        }
    }
}
