using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDashboardRepository
    {
        Task<DashboardResource> GetDashboard();
        
        /// <param name="projects"></param>
        /// <param name="environments"></param>
        /// <param name="withLatestSuccessfulRelease">includes latest successful release in the list of Items</param>
        Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments, bool withLatestSuccessfulRelease = false);
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

        public async Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments, bool withLatestSuccessfulRelease = false)
        {
            var includePrevious = withLatestSuccessfulRelease;
            return await repository.Client.Get<DashboardResource>(await repository.Link("DashboardDynamic").ConfigureAwait(false), new { projects, environments, includePrevious }).ConfigureAwait(false);
        }
    }
}
