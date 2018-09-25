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
        private readonly IOctopusAsyncRepository repository;

        public FeaturesConfigurationRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public Task<FeaturesConfigurationResource> GetFeaturesConfiguration()
        {
            return repository.Client.Get<FeaturesConfigurationResource>(repository.Link("FeaturesConfiguration"));
        }

        public Task<FeaturesConfigurationResource> ModifyFeaturesConfiguration(FeaturesConfigurationResource resource)
        {
            return repository.Client.Update(repository.Link("FeaturesConfiguration"), resource);
        }
    }
}
