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
        private readonly IOctopusAsyncRepository repository;

        public DashboardRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public Task<DashboardResource> GetDashboard()
        {
            return repository.Client.Get<DashboardResource>(repository.Link("Dashboard"));
        }

        public Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments)
        {
            return repository.Client.Get<DashboardResource>(repository.Link("DashboardDynamic"), new { projects, environments });
        }
    }
}
