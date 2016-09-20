using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IServerStatusRepository
    {
        Task<ServerStatusResource> GetServerStatus();
        Task<SystemInfoResource> GetSystemInfo(ServerStatusResource status);
    }

    class ServerStatusRepository : BasicRepository<ServerStatusResource>, IServerStatusRepository
    {
        public ServerStatusRepository(IOctopusAsyncClient client)
            : base(client, null) // Not a collection
        {
        }

        public Task<ServerStatusResource> GetServerStatus()
        {
            return Client.Get<ServerStatusResource>(Client.RootDocument.Link("ServerStatus"));
        }

        public Task<SystemInfoResource> GetSystemInfo(ServerStatusResource status)
        {
            if (status == null) throw new ArgumentNullException("status");
            return Client.Get<SystemInfoResource>(status.Link("SystemInfo"));
        }
    }
}
