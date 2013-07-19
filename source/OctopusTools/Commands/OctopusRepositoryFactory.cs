using System;
using Octopus.Client;

namespace OctopusTools.Commands
{
    class OctopusRepositoryFactory : IOctopusRepositoryFactory
    {
        public IOctopusRepository CreateRepository(OctopusServerEndpoint endpoint)
        {
            return new OctopusRepository(endpoint);
        }
    }
}