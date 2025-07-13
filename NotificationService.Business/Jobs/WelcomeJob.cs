using NotificationService.Business.Interfaces;
using Quartz;

namespace NotificationService.Business.Jobs;

public class WelcomeEmailJob : IJob
{
    private readonly IEmailService _emailService;

    public WelcomeEmailJob(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var userId = Guid.Parse(context.MergedJobDataMap.GetString("UserId")!);
        var userEmail = context.MergedJobDataMap.GetString("Email")!;
        var userName = context.MergedJobDataMap.GetString("Name")!;

        await _emailService.SendWelcomeEmail(userEmail, userName);
    }
}
