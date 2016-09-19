using Octopus.Client;

namespace Octopus.Cli.Repositories
{
    public interface IOctopusRepositoryFactory
    {
        IOctopusRepository CreateRepository(IOctopusClient client);
    }

    public class OctopusRepositoryFactory : IOctopusRepositoryFactory
    {
        public IOctopusRepository CreateRepository(IOctopusClient client)
        {
            return new OctopusRepository(client);
        }
    }
}