using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDashboardConfigurationRepository
    {
        DashboardConfigurationResource GetDashboardConfiguration();
        DashboardConfigurationResource ModifyDashboardConfiguration(DashboardConfigurationResource resource);
    }
    
    class DashboardConfigurationRepository : IDashboardConfigurationRepository
    {
        readonly IOctopusClient client;

        public DashboardConfigurationRepository(IOctopusClient client)
        {
            this.client = client;
        }

        public DashboardConfigurationResource GetDashboardConfiguration()
        {
            return client.Get<DashboardConfigurationResource>(client.Link("DashboardConfiguration"));
        }

        public DashboardConfigurationResource ModifyDashboardConfiguration(DashboardConfigurationResource resource)
        {
            return client.Update(client.Link("DashboardConfiguration"), resource);
        }
    }
}