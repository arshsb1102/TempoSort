using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

public class JobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public JobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob
            ?? throw new ArgumentNullException($"Cannot create job of type {bundle.JobDetail.JobType}");
    }

    public void ReturnJob(IJob job) { }
}
