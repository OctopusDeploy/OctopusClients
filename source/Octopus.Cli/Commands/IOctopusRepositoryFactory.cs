using System;
using Octopus.Client;

namespace Octopus.Cli.Commands
{
    public interface IOctopusRepositoryFactory
    {
        IOctopusRepository CreateRepository(OctopusServerEndpoint endpoint);
    }
}