using System.Threading.Tasks;
using Octopus.Client;

namespace Octopus.Cli.Repositories
{
    public interface IOctopusAsyncRepositoryFactory
    {
        IOctopusAsyncRepository CreateRepository(IOctopusAsyncClient client, SpaceContext spaceContext = null);
    }

    public class OctopusRepositoryFactory : IOctopusAsyncRepositoryFactory
    {
        public IOctopusAsyncRepository CreateRepository(IOctopusAsyncClient client, SpaceContext spaceContext = null)
        {
            return client.CreateRepository(spaceContext);
        }
    }
}