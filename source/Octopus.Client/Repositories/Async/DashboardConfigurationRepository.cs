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
        readonly IOctopusAsyncClient client;

        public DashboardConfigurationRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public Task<DashboardConfigurationResource> GetDashboardConfiguration()
        {
            return client.Get<DashboardConfigurationResource>(client.Link("DashboardConfiguration"));
        }

        public Task<DashboardConfigurationResource> ModifyDashboardConfiguration(DashboardConfigurationResource resource)
        {
            return client.Update(client.Link("DashboardConfiguration"), resource);
        }
    }
}
