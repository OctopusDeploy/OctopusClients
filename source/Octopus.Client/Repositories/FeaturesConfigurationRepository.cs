using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IFeaturesConfigurationRepository
    {
        FeaturesConfigurationResource GetFeaturesConfiguration();
        FeaturesConfigurationResource ModifyFeaturesConfiguration(FeaturesConfigurationResource resource);
    }
    
    class FeaturesConfigurationRepository : IFeaturesConfigurationRepository
    {
        readonly IOctopusClient client;

        public FeaturesConfigurationRepository(IOctopusClient client)
        {
            this.client = client;
        }

        public FeaturesConfigurationResource GetFeaturesConfiguration()
        {
            return client.Get<FeaturesConfigurationResource>(client.Link("FeaturesConfiguration"));
        }

        public FeaturesConfigurationResource ModifyFeaturesConfiguration(FeaturesConfigurationResource resource)
        {
            return client.Update(client.Link("FeaturesConfiguration"), resource);
        }
    }
}