using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduledEmailController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public ScheduledEmailController(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        [HttpPost("send-after")]
        public async Task<IActionResult> SendEmailAfterMinutes([FromQuery] int minutes, [FromBody] EmailRequest request)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var job = JobBuilder.Create<NotificationService.Business.Jobs.EmailJob>()
                .WithIdentity(Guid.NewGuid().ToString())
                .UsingJobData("to", request.To)
                .UsingJobData("subject", request.Subject)
                .UsingJobData("body", request.Body)
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartAt(DateTimeOffset.UtcNow.AddMinutes(minutes))
                .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
            return Ok(new { message = $"Email scheduled after {minutes} minute(s)" });
        }
    }

    public class EmailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
