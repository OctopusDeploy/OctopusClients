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
        private readonly IOctopusRepository repository;

        public FeaturesConfigurationRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public FeaturesConfigurationResource GetFeaturesConfiguration()
        {
            return repository.Client.Get<FeaturesConfigurationResource>(repository.Link("FeaturesConfiguration"));
        }

        public FeaturesConfigurationResource ModifyFeaturesConfiguration(FeaturesConfigurationResource resource)
        {
            return repository.Client.Update(repository.Link("FeaturesConfiguration"), resource);
        }
    }
}