using Octopus.Client;

namespace Octopus.Cli.Repositories
{
    public interface IOctopusAsyncRepositoryFactory
    {
        IOctopusAsyncRepository CreateRepository(IOctopusAsyncClient client);
    }

    public class OctopusRepositoryFactory : IOctopusAsyncRepositoryFactory
    {
        public IOctopusAsyncRepository CreateRepository(IOctopusAsyncClient client)
        {
            return client.CreateRepository();
        }
    }
}