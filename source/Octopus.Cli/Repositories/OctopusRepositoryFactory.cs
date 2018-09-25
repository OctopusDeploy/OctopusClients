using System.Threading.Tasks;
using Octopus.Client;

namespace Octopus.Cli.Repositories
{
    public interface IOctopusAsyncRepositoryFactory
    {
        Task<IOctopusAsyncRepository> CreateRepository(IOctopusAsyncClient client);
    }

    public class OctopusRepositoryFactory : IOctopusAsyncRepositoryFactory
    {
        public Task<IOctopusAsyncRepository> CreateRepository(IOctopusAsyncClient client)
        {
            return client.CreateRepository();
        }
    }
}