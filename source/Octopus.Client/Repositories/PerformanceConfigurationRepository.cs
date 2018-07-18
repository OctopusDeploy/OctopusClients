using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IPerformanceConfigurationRepository
    {
        PerformanceConfigurationResource Get();
        PerformanceConfigurationResource Modify(PerformanceConfigurationResource resource);
    }

    public class PerformanceConfigurationRepository : IPerformanceConfigurationRepository
    {
        readonly IOctopusClient client;

        public PerformanceConfigurationRepository(IOctopusClient client)
        {
            this.client = client;
        }

        public PerformanceConfigurationResource Get()
        {
            return client.Get<PerformanceConfigurationResource>(client.RootDocument.Link("PerformanceConfiguration"));
        }

        public PerformanceConfigurationResource Modify(PerformanceConfigurationResource resource)
        {
            return client.Update(client.RootDocument.Link("PerformanceConfiguration"), resource);
        }
    }
}