using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IServerStatusRepository
    {
        ServerStatusResource GetServerStatus();
        SystemInfoResource GetSystemInfo(ServerStatusResource status);
        ServerStatusHealthResource GetServerHealth();
    }

    class ServerStatusRepository : BasicRepository<ServerStatusResource>, IServerStatusRepository
    {
        public ServerStatusRepository(IOctopusRepository repository)
            : base(repository, null) // Not a collection
        {
        }

        public ServerStatusResource GetServerStatus()
        {
            return Client.Get<ServerStatusResource>(Repository.Link("ServerStatus"));
        }

        public ServerStatusHealthResource GetServerHealth()
        {
            return Client.Get<ServerStatusHealthResource>(Repository.Link("ServerHealthStatus"));
        }

        public SystemInfoResource GetSystemInfo(ServerStatusResource status)
        {
            if (status == null) throw new ArgumentNullException("status");
            return Client.Get<SystemInfoResource>(status.Link("SystemInfo"));
        }
    }
}