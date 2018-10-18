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

        public async Task<FeaturesConfigurationResource> GetFeaturesConfiguration()
        {
            return await repository.Client.Get<FeaturesConfigurationResource>(await repository.Link("FeaturesConfiguration"));
        }

        public async Task<FeaturesConfigurationResource> ModifyFeaturesConfiguration(FeaturesConfigurationResource resource)
        {
            return await repository.Client.Update(await repository.Link("FeaturesConfiguration"), resource);
        }
    }
}
