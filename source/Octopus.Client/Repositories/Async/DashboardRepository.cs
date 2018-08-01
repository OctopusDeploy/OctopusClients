using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDashboardRepository
    {
        Task<DashboardResource> GetDashboard();
        Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments);
    }

    class DashboardRepository : IDashboardRepository
    {
        readonly IOctopusAsyncClient client;

        public DashboardRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public Task<DashboardResource> GetDashboard()
        {
            return client.Get<DashboardResource>(client.Link("Dashboard"));
        }

        public Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments)
        {
            return client.Get<DashboardResource>(client.Link("DashboardDynamic"), new { projects, environments });
        }
    }
}
