using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDashboardConfigurationRepository
    {
        Task<DashboardConfigurationResource> GetDashboardConfiguration();
        Task<DashboardConfigurationResource> ModifyDashboardConfiguration(DashboardConfigurationResource resource);
    }

    class DashboardConfigurationRepository : IDashboardConfigurationRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public DashboardConfigurationRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public Task<DashboardConfigurationResource> GetDashboardConfiguration()
        {
            return repository.Client.Get<DashboardConfigurationResource>(repository.Link("DashboardConfiguration"));
        }

        public Task<DashboardConfigurationResource> ModifyDashboardConfiguration(DashboardConfigurationResource resource)
        {
            return repository.Client.Update(repository.Link("DashboardConfiguration"), resource);
        }
    }
}
