using System;
using Octopus.Client;

namespace Octopus.Cli.Commands
{
    class OctopusRepositoryFactory : IOctopusRepositoryFactory
    {
        public IOctopusRepository CreateRepository(OctopusServerEndpoint endpoint)
        {
            return new OctopusRepository(endpoint);
        }
    }
}