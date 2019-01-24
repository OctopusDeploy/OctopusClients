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
        private readonly IOctopusRepository repository;

        public PerformanceConfigurationRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public PerformanceConfigurationResource Get()
        {
            return repository.Client.Get<PerformanceConfigurationResource>(repository.Link("PerformanceConfiguration"));
        }

        public PerformanceConfigurationResource Modify(PerformanceConfigurationResource resource)
        {
            return repository.Client.Update(repository.Link("PerformanceConfiguration"), resource);
        }
    }
}