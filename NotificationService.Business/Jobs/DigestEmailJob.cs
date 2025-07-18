using Quartz;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Business.Interfaces;

namespace NotificationService.Business.Jobs;

public class DigestEmailJob : IJob
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public DigestEmailJob(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var nowUtc = DateTime.UtcNow;

        var users = await _userRepository.GetUsersForDigestAsync(nowUtc.Hour, nowUtc.Minute);

        foreach (var user in users)
        {
            await _emailService.SendDailyDigest(user.Email, user.Name, user.UserId);
        }
    }
}
