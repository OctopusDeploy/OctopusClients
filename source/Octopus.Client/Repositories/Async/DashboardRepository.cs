using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDashboardRepository
    {
        Task<DashboardResource> GetDashboard();
        Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments, bool includePrevious);
    }

    class DashboardRepository : IDashboardRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public DashboardRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public async Task<DashboardResource> GetDashboard()
        {
            return await repository.Client.Get<DashboardResource>(await repository.Link("Dashboard").ConfigureAwait(false)).ConfigureAwait(false);
        }

        public async Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments, bool includePrevious = true)
        {
            return await repository.Client.Get<DashboardResource>(await repository.Link("DashboardDynamic").ConfigureAwait(false), new { projects, environments, includePrevious }).ConfigureAwait(false);
        }
    }
}
