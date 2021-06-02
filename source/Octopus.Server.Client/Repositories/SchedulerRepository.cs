using System.IO;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ISchedulerRepository
    {
        void Start();
        void Start(string taskName);
        void Stop();
        void Stop(string taskName);
        void Trigger(string taskName);
        ScheduledTaskDetailsResource GetLogs(string taskName);
        Stream GetRawLogs(string taskName);
        SchedulerStatusResource Status();
    }
    
    class SchedulerRepository : ISchedulerRepository
    {
        private readonly IOctopusRepository repository;

        public SchedulerRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public void Start()
        {
            repository.Client.GetContent("~/api/scheduler/start");
        }

        public void Start(string taskName)
        {
            repository.Client.GetContent($"~/api/scheduler/start?task={taskName}");
        }

        public void Trigger(string taskName)
        {
            repository.Client.GetContent($"~/api/scheduler/trigger?task={taskName}");
        }

        public void Stop()
        {
            repository.Client.GetContent("~/api/scheduler/stop");
        }

        public void Stop(string taskName)
        {
            repository.Client.GetContent($"~/api/scheduler/stop?task={taskName}");
        }

        public ScheduledTaskDetailsResource GetLogs(string taskName)
        {
            return repository.Client.Get<ScheduledTaskDetailsResource>($"~/api/scheduler/{taskName}/logs");
        }

        public Stream GetRawLogs(string taskName)
        {
            return repository.Client.GetContent($"~/api/scheduler/{taskName}/logs/raw");
        }

        public SchedulerStatusResource Status()
        {
            return repository.Client.Get<SchedulerStatusResource>("~/api/scheduler");
        }
    }
}