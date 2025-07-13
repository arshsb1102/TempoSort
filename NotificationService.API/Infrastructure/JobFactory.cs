using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
namespace NotificationService.API.Infrastructure;

public class ScopedJobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ScopedJobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var scope = _serviceProvider.CreateScope();

        try
        {
            var job = scope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;

            if (job == null)
                throw new InvalidOperationException($"Cannot create job of type {bundle.JobDetail.JobType}");

            return new ScopedJobWrapper(job, scope);
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }

    public void ReturnJob(IJob job)
    {
        if (job is ScopedJobWrapper wrapper)
        {
            (wrapper.InnerJob as IDisposable)?.Dispose();
            wrapper.Scope.Dispose();
        }
    }

    private class ScopedJobWrapper : IJob
    {
        public IJob InnerJob { get; }
        public IServiceScope Scope { get; }

        public ScopedJobWrapper(IJob innerJob, IServiceScope scope)
        {
            InnerJob = innerJob;
            Scope = scope;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return InnerJob.Execute(context);
        }
    }
}
