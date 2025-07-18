using NotificationService.Business.Interfaces;
using NotificationService.DataAccess.Interfaces;
using Quartz;

namespace NotificationService.Business.Jobs;

public class WelcomeEmailJob : IJob
{
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;

    public WelcomeEmailJob(IEmailService emailService, IUserRepository userRepository)
    {
        _emailService = emailService;
        _userRepository = userRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var userId = Guid.Parse(context.MergedJobDataMap.GetString("UserId")!);
        var userEmail = context.MergedJobDataMap.GetString("Email")!;
        var userName = context.MergedJobDataMap.GetString("Name")!;
        await _emailService.SendWelcomeEmail(userEmail, userName);
        await _userRepository.UpdateWelcomeOnAsync(userId);
    }
}
