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

        public async Task<DashboardConfigurationResource> GetDashboardConfiguration()
        {
            return await repository.Client.Get<DashboardConfigurationResource>(await repository.Link("DashboardConfiguration"));
        }

        public async Task<DashboardConfigurationResource> ModifyDashboardConfiguration(DashboardConfigurationResource resource)
        {
            return await repository.Client.Update(await repository.Link("DashboardConfiguration"), resource);
        }
    }
}
