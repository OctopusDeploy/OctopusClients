using System;

namespace Octopus.Client.Repositories
{
    public interface ISchedulerRepository
    {
        void Start();
        void Start(string taskName);
        void Stop();
        void Stop(string taskName);
        void Trigger(string taskName);
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
    }
}