using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IPerformanceConfigurationRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<PerformanceConfigurationResource> Get();
        Task<PerformanceConfigurationResource> Get(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<PerformanceConfigurationResource> Modify(PerformanceConfigurationResource resource);
        Task<PerformanceConfigurationResource> Modify(PerformanceConfigurationResource resource, CancellationToken cancellationToken);
    }

    class PerformanceConfigurationRepository : IPerformanceConfigurationRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public PerformanceConfigurationRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<PerformanceConfigurationResource> Get()
            => Get(CancellationToken.None);

        public async Task<PerformanceConfigurationResource> Get(CancellationToken cancellationToken)
        {
            return await repository.Client.Get<PerformanceConfigurationResource>(await repository.Link("PerformanceConfiguration").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<PerformanceConfigurationResource> Modify(PerformanceConfigurationResource resource)
            => Modify(resource, CancellationToken.None);

        public async Task<PerformanceConfigurationResource> Modify(PerformanceConfigurationResource resource, CancellationToken cancellationToken)
        {
            return await repository.Client.Update(await repository.Link("PerformanceConfiguration").ConfigureAwait(false), resource, cancellationToken).ConfigureAwait(false);
        }
    }
}
