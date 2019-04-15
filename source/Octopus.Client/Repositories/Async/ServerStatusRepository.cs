using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IServerStatusRepository
    {
        Task<ServerStatusResource> GetServerStatus();
        Task<SystemInfoResource> GetSystemInfo(ServerStatusResource status);
        Task<ServerStatusHealthResource> GetServerHealth(ServerStatusResource status);
    }

    class ServerStatusRepository : BasicRepository<ServerStatusResource>, IServerStatusRepository
    {
        public ServerStatusRepository(IOctopusAsyncRepository repository)
            : base(repository, "") // Not a collection
        {
        }

        public async Task<ServerStatusResource> GetServerStatus()
        {
            return await Client.Get<ServerStatusResource>(await Repository.Link("ServerStatus").ConfigureAwait(false)).ConfigureAwait(false);
        }

        public Task<SystemInfoResource> GetSystemInfo(ServerStatusResource status)
        {
            if (status == null) throw new ArgumentNullException("status");
            return Client.Get<SystemInfoResource>(status.Link("SystemInfo"));
        }

        public Task<ServerStatusHealthResource> GetServerHealth(ServerStatusResource status)
        {
            if (status == null) throw new ArgumentNullException("status");
            return Client.Get<ServerStatusHealthResource>(status.Link("Health"));
        }
    }
}
