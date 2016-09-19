using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IProxyRepository : IGet<ProxyResource>, ICreate<ProxyResource>, IModify<ProxyResource>, IDelete<ProxyResource>, IFindByName<ProxyResource>
    {
    }
}