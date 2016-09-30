using System;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface ISchedulerRepository
    {
        Task Start();
        Task Start(string taskName);
        Task Stop();
        Task Stop(string taskName);
        Task Trigger(string taskName);
    }

    class SchedulerRepository : ISchedulerRepository
    {
        readonly IOctopusAsyncClient client;

        public SchedulerRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public Task Start()
        {
            return client.GetContent("~/api/scheduler/start");
        }

        public Task Start(string taskName)
        {
            return client.GetContent($"~/api/scheduler/start?task={taskName}");
        }

        public Task Trigger(string taskName)
        {
            return client.GetContent($"~/api/scheduler/trigger?task={taskName}");
        }

        public Task Stop()
        {
            return client.GetContent("~/api/scheduler/stop");
        }

        public Task Stop(string taskName)
        {
            return client.GetContent($"~/api/scheduler/stop?task={taskName}");
        }
    }
}
