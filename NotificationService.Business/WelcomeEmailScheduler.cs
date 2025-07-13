using NotificationService.Business.Interfaces;
using NotificationService.Business.Jobs;
using NotificationService.Models.DBObjects;
using Quartz;


namespace NotificationService.Business;

public class WelcomeEmailScheduler : IWelcomeEmailScheduler
{
    private readonly ISchedulerFactory _schedulerFactory;

    public WelcomeEmailScheduler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task ScheduleAsync(User user, int delayInMinutes)
    {

        var scheduler = await _schedulerFactory.GetScheduler();
        var userId = user.UserId;
        var userEmail = user.Email;
        var jobKey = new JobKey($"WelcomeEmailJob-{user.UserId}", "EmailJobs");
        bool exists = await scheduler.CheckExists(jobKey);
        if (exists)
        {
            return;
        }
        var job = JobBuilder.Create<WelcomeEmailJob>()
            .WithIdentity($"WelcomeEmailJob-{userId}", "EmailJobs")
            .UsingJobData("UserId", userId.ToString())
            .UsingJobData("Email", userEmail)
            .UsingJobData("Name", user.Name)
            .Build();

        var trigger = TriggerBuilder.Create()
            .StartAt(DateTimeOffset.UtcNow.AddMinutes(delayInMinutes))
            .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow())
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}
