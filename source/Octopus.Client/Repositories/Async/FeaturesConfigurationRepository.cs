using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IFeaturesConfigurationRepository
    {
        Task<FeaturesConfigurationResource> GetFeaturesConfiguration();
        Task<FeaturesConfigurationResource> ModifyFeaturesConfiguration(FeaturesConfigurationResource resource);
    }

    class FeaturesConfigurationRepository : IFeaturesConfigurationRepository
    {
        readonly IOctopusAsyncClient client;

        public FeaturesConfigurationRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public Task<FeaturesConfigurationResource> GetFeaturesConfiguration()
        {
            return client.Get<FeaturesConfigurationResource>(client.RootDocument.Link("FeaturesConfiguration"));
        }

        public Task<FeaturesConfigurationResource> ModifyFeaturesConfiguration(FeaturesConfigurationResource resource)
        {
            return client.Update(client.RootDocument.Link("FeaturesConfiguration"), resource);
        }
    }
}
