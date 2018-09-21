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
        readonly IOctopusClient client;

        public SchedulerRepository(IOctopusClient client)
        {
            this.client = client;
        }

        public void Start()
        {
            client.GetContent("~/api/scheduler/start");
        }

        public void Start(string taskName)
        {
            client.GetContent($"~/api/scheduler/start?task={taskName}");
        }

        public void Trigger(string taskName)
        {
            client.GetContent($"~/api/scheduler/trigger?task={taskName}");
        }

        public void Stop()
        {
            client.GetContent("~/api/scheduler/stop");
        }

        public void Stop(string taskName)
        {
            client.GetContent($"~/api/scheduler/stop?task={taskName}");
        }

        public ScheduledTaskDetailsResource GetLogs(string taskName)
        {
            return client.Get<ScheduledTaskDetailsResource>($"~/api/scheduler/{taskName}/logs");
        }

        public Stream GetRawLogs(string taskName)
        {
            return client.GetContent($"~/api/scheduler/{taskName}/logs/raw");
        }

        public SchedulerStatusResource Status()
        {
            return client.Get<SchedulerStatusResource>("~/api/scheduler");
        }
    }
}