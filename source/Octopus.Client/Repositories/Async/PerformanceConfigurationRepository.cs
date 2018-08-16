using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IPerformanceConfigurationRepository
    {
        Task<PerformanceConfigurationResource> Get();
        Task<PerformanceConfigurationResource> Modify(PerformanceConfigurationResource resource);
    }

    class PerformanceConfigurationRepository : IPerformanceConfigurationRepository
    {
        readonly IOctopusAsyncClient client;

        public PerformanceConfigurationRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public Task<PerformanceConfigurationResource> Get()
        {
            return client.Get<PerformanceConfigurationResource>(client.RootDocument.Link("PerformanceConfiguration"));
        }

        public Task<PerformanceConfigurationResource> Modify(PerformanceConfigurationResource resource)
        {
            return client.Update(client.RootDocument.Link("PerformanceConfiguration"), resource);
        }
    }
}