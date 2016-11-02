using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IProxyRepository : IGet<ProxyResource>, ICreate<ProxyResource>, IModify<ProxyResource>, IDelete<ProxyResource>, IFindByName<ProxyResource>
    {
    }
}