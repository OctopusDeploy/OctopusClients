using System.Threading.Tasks;

namespace Octopus.Client.Repositories
{
    public interface ISchedulerRepository
    {
        Task Start();
        Task Start(string taskName);
        Task Stop();
        Task Stop(string taskName);
        Task Trigger(string taskName);
    }
}
