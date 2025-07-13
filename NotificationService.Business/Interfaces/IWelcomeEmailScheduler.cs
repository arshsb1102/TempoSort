using NotificationService.Models.DBObjects;

namespace NotificationService.Business.Interfaces;

public interface IWelcomeEmailScheduler
{
    Task ScheduleAsync(User user, int delayInMinutes);
}
