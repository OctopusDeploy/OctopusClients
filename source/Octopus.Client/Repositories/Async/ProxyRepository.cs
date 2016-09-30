using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    class ProxyRepository : BasicRepository<ProxyResource>, IProxyRepository
    {
        public ProxyRepository(IOctopusAsyncClient client)
            : base(client, "Proxies")
        {

        }
    }
}