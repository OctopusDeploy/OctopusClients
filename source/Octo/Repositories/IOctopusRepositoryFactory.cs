using Octopus.Client;

namespace Octopus.Cli.Repositories
{
    public interface IOctopusRepositoryFactory
    {
        IOctopusRepository CreateRepository(OctopusServerEndpoint endpoint);
        IOctopusRepository CreateRepository(OctopusServerEndpoint endpoint, OctopusClientOptions options);
    }
}