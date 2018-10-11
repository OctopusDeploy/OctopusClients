using Octopus.Client.Repositories.Async;

namespace Octopus.Client
{
    public interface IAsyncMixedScopeRepository
    {
        IEventRepository Events { get; }
        ITaskRepository Tasks { get; }
        ITeamsRepository Teams { get; }
    }
}