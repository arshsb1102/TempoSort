using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Business.Interfaces;
using NotificationService.Models.DTOs;
using System.Security.Claims;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var userId = GetUserId();
        var taskId = await _taskService.CreateTaskAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id = taskId }, new { taskId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var tasks = await _taskService.GetTasksAsync(userId);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        var task = await _taskService.GetTaskByIdAsync(userId, id);
        return task is not null ? Ok(task) : NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
    {
        var userId = GetUserId();
        await _taskService.UpdateTaskAsync(userId, id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        await _taskService.DeleteTaskAsync(userId, id);
        return NoContent();
    }

    [HttpPost("{id}/toggle-complete")]
    public async Task<IActionResult> ToggleComplete(Guid id)
    {
        var userId = GetUserId();
        await _taskService.ToggleCompleteAsync(userId, id);
        return NoContent();
    }
}
