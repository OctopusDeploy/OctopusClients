using System;
using Octopus.Client;

namespace OctopusTools.Commands
{
    public interface IOctopusRepositoryFactory
    {
        IOctopusRepository CreateRepository(OctopusServerEndpoint endpoint);
    }
}