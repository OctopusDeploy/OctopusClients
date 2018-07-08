using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDashboardRepository
    {
        DashboardResource GetDashboard();
        DashboardResource GetDynamicDashboard(string[] projects, string[] environments);
    }
    
    class DashboardRepository : IDashboardRepository
    {
        readonly IOctopusClient client;

        public DashboardRepository(IOctopusClient client)
        {
            this.client = client;
        }

        public DashboardResource GetDashboard()
        {
            return client.Get<DashboardResource>(client.Link("Dashboard"));
        }

        public DashboardResource GetDynamicDashboard(string[] projects, string[] environments)
        {
            return client.Get<DashboardResource>(client.Link("DashboardDynamic"), new { projects, environments });
        }
    }
}