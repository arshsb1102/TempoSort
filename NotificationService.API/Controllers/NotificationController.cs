using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Business.Interfaces;
using NotificationService.Models;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationHelper _service;

    public NotificationsController(INotificationHelper service)
    {
        _service = service;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NotificationCreateDto dto)
    {
        await _service.CreateAsync(GetUserId(), dto.Message, dto.Link, dto.Type);
        return Ok(new { message = "Notification created." });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync(GetUserId());
        return Ok(items);
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnread()
    {
        var items = await _service.GetUnreadAsync(GetUserId());
        return Ok(items);
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _service.MarkAsReadAsync(id, GetUserId());
        return Ok(new { message = "Marked as read." });
    }
}
