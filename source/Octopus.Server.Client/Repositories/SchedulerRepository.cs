using System.IO;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ISchedulerRepository
    {
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
