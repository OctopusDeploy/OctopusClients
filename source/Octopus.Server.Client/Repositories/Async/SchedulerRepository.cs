using System;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ISchedulerRepository
    {
        Task Start();
        Task Start(string taskName);
        Task Stop();
        Task Stop(string taskName);
        Task Trigger(string taskName);
        Task<ScheduledTaskDetailsResource> GetLogs(string taskName);
        Task<Stream> GetRawLogs(string taskName);
        Task<SchedulerStatusResource> Status();
    }

    class SchedulerRepository : ISchedulerRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public SchedulerRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public Task Start()
        {
            return repository.Client.GetContent("~/api/scheduler/start");
        }

        public Task Start(string taskName)
        {
            return repository.Client.GetContent($"~/api/scheduler/start?task={taskName}");
        }

        public Task Trigger(string taskName)
        {
            return repository.Client.GetContent($"~/api/scheduler/trigger?task={taskName}");
        }

        public Task Stop()
        {
            return repository.Client.GetContent("~/api/scheduler/stop");
        }

        public Task Stop(string taskName)
        {
            return repository.Client.GetContent($"~/api/scheduler/stop?task={taskName}");
        }

        public Task<ScheduledTaskDetailsResource> GetLogs(string taskName)
        {
            return repository.Client.Get<ScheduledTaskDetailsResource>($"~/api/scheduler/{taskName}/logs");
        }

        public Task<Stream> GetRawLogs(string taskName)
        {
            return repository.Client.GetContent($"~/api/scheduler/{taskName}/logs/raw");
        }

        public Task<SchedulerStatusResource> Status()
        {
            return repository.Client.Get<SchedulerStatusResource>("~/api/scheduler");
        }
    }
}
