using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IServerStatusRepository
    {
        ServerStatusResource GetServerStatus();
        SystemInfoResource GetSystemInfo(ServerStatusResource status);
    }
}