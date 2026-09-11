using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IServerStatusRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ServerStatusResource> GetServerStatus();
        Task<ServerStatusResource> GetServerStatus(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<SystemInfoResource> GetSystemInfo(ServerStatusResource status);
        Task<SystemInfoResource> GetSystemInfo(ServerStatusResource status, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ServerStatusHealthResource> GetServerHealth();
        Task<ServerStatusHealthResource> GetServerHealth(CancellationToken cancellationToken);
    }

    class ServerStatusRepository : BasicRepository<ServerStatusResource>, IServerStatusRepository
    {
        public ServerStatusRepository(IOctopusAsyncRepository repository)
            : base(repository, "") // Not a collection
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ServerStatusResource> GetServerStatus()
            => GetServerStatus(CancellationToken.None);

        public async Task<ServerStatusResource> GetServerStatus(CancellationToken cancellationToken)
        {
            return await Client.Get<ServerStatusResource>(await Repository.Link("ServerStatus").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<SystemInfoResource> GetSystemInfo(ServerStatusResource status)
            => GetSystemInfo(status, CancellationToken.None);

        public Task<SystemInfoResource> GetSystemInfo(ServerStatusResource status, CancellationToken cancellationToken)
        {
            if (status == null) throw new ArgumentNullException("status");
            return Client.Get<SystemInfoResource>(status.Link("SystemInfo"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ServerStatusHealthResource> GetServerHealth()
            => GetServerHealth(CancellationToken.None);

        public async Task<ServerStatusHealthResource> GetServerHealth(CancellationToken cancellationToken)
        {
            return await Client.Get<ServerStatusHealthResource>(await Repository.Link("ServerHealthStatus").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }
    }
}
