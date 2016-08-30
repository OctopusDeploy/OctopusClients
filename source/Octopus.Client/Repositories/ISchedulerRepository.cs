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
}
