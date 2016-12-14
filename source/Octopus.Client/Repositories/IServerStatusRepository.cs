using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IServerStatusRepository
    {
        ServerStatusResource GetServerStatus();
        SystemInfoResource GetSystemInfo(ServerStatusResource status);
    }
    
    class ServerStatusRepository : BasicRepository<ServerStatusResource>, IServerStatusRepository
    {
        public ServerStatusRepository(IOctopusClient client)
            : base(client, null) // Not a collection
        {
        }

        public ServerStatusResource GetServerStatus()
        {
            return Client.Get<ServerStatusResource>(Client.RootDocument.Link("ServerStatus"));
        }

        public SystemInfoResource GetSystemInfo(ServerStatusResource status)
        {
            if (status == null) throw new ArgumentNullException("status");
            return Client.Get<SystemInfoResource>(status.Link("SystemInfo"));
        }
    }
}