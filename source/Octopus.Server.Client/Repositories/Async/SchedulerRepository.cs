using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ISchedulerRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Start();
        Task Start(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Start(string taskName);
        Task Start(string taskName, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Stop();
        Task Stop(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Stop(string taskName);
        Task Stop(string taskName, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Trigger(string taskName);
        Task Trigger(string taskName, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ScheduledTaskDetailsResource> GetLogs(string taskName);
        Task<ScheduledTaskDetailsResource> GetLogs(string taskName, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<Stream> GetRawLogs(string taskName);
        Task<Stream> GetRawLogs(string taskName, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<SchedulerStatusResource> Status();
        Task<SchedulerStatusResource> Status(CancellationToken cancellationToken);
    }

    class SchedulerRepository : ISchedulerRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public SchedulerRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Start()
            => Start(CancellationToken.None);

        public Task Start(CancellationToken cancellationToken)
        {
            return repository.Client.GetContent("~/api/scheduler/start", cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Start(string taskName)
            => Start(taskName, CancellationToken.None);

        public Task Start(string taskName, CancellationToken cancellationToken)
        {
            return repository.Client.GetContent($"~/api/scheduler/start?task={taskName}", cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Trigger(string taskName)
            => Trigger(taskName, CancellationToken.None);

        public Task Trigger(string taskName, CancellationToken cancellationToken)
        {
            return repository.Client.GetContent($"~/api/scheduler/trigger?task={taskName}", cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Stop()
            => Stop(CancellationToken.None);

        public Task Stop(CancellationToken cancellationToken)
        {
            return repository.Client.GetContent("~/api/scheduler/stop", cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Stop(string taskName)
            => Stop(taskName, CancellationToken.None);

        public Task Stop(string taskName, CancellationToken cancellationToken)
        {
            return repository.Client.GetContent($"~/api/scheduler/stop?task={taskName}", cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ScheduledTaskDetailsResource> GetLogs(string taskName)
            => GetLogs(taskName, CancellationToken.None);

        public Task<ScheduledTaskDetailsResource> GetLogs(string taskName, CancellationToken cancellationToken)
        {
            return repository.Client.Get<ScheduledTaskDetailsResource>($"~/api/scheduler/{taskName}/logs", cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<Stream> GetRawLogs(string taskName)
            => GetRawLogs(taskName, CancellationToken.None);

        public Task<Stream> GetRawLogs(string taskName, CancellationToken cancellationToken)
        {
            return repository.Client.GetContent($"~/api/scheduler/{taskName}/logs/raw", cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<SchedulerStatusResource> Status()
            => Status(CancellationToken.None);

        public Task<SchedulerStatusResource> Status(CancellationToken cancellationToken)
        {
            return repository.Client.Get<SchedulerStatusResource>("~/api/scheduler", cancellationToken);
        }
    }
}
