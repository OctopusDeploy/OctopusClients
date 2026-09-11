using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IFeaturesConfigurationRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<FeaturesConfigurationResource> GetFeaturesConfiguration();
        Task<FeaturesConfigurationResource> GetFeaturesConfiguration(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<FeaturesConfigurationResource> ModifyFeaturesConfiguration(FeaturesConfigurationResource resource);
        Task<FeaturesConfigurationResource> ModifyFeaturesConfiguration(FeaturesConfigurationResource resource, CancellationToken cancellationToken);
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
            return GetFeaturesConfiguration(CancellationToken.None);
        }

        public async Task<FeaturesConfigurationResource> GetFeaturesConfiguration(CancellationToken cancellationToken)
        {
            return await repository.Client.Get<FeaturesConfigurationResource>(await repository.Link("FeaturesConfiguration").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        public Task<FeaturesConfigurationResource> ModifyFeaturesConfiguration(FeaturesConfigurationResource resource)
        {
            return ModifyFeaturesConfiguration(resource, CancellationToken.None);
        }

        public async Task<FeaturesConfigurationResource> ModifyFeaturesConfiguration(FeaturesConfigurationResource resource, CancellationToken cancellationToken)
        {
            return await repository.Client.Update(await repository.Link("FeaturesConfiguration").ConfigureAwait(false), resource, cancellationToken).ConfigureAwait(false);
        }
    }
}
