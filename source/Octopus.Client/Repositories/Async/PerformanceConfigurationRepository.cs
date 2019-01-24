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
        private readonly IOctopusAsyncRepository repository;

        public PerformanceConfigurationRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PerformanceConfigurationResource> Get()
        {
            return await repository.Client.Get<PerformanceConfigurationResource>(await repository.Link("PerformanceConfiguration").ConfigureAwait(false)).ConfigureAwait(false);
        }

        public async Task<PerformanceConfigurationResource> Modify(PerformanceConfigurationResource resource)
        {
            return await repository.Client.Update(await repository.Link("PerformanceConfiguration").ConfigureAwait(false), resource).ConfigureAwait(false);
        }
    }
}