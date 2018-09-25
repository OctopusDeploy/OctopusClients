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

        public Task<PerformanceConfigurationResource> Get()
        {
            return repository.Client.Get<PerformanceConfigurationResource>(repository.Link("PerformanceConfiguration"));
        }

        public Task<PerformanceConfigurationResource> Modify(PerformanceConfigurationResource resource)
        {
            return repository.Client.Update(repository.Link("PerformanceConfiguration"), resource);
        }
    }
}