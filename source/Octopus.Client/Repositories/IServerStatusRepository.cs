using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IServerStatusRepository
    {
        Task<ServerStatusResource> GetServerStatus();
        Task<SystemInfoResource> GetSystemInfo(ServerStatusResource status);
    }
}