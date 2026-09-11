using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDashboardRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DashboardResource> GetDashboard();
        Task<DashboardResource> GetDashboard(CancellationToken cancellationToken);

        /// <param name="projects"></param>
        /// <param name="environments"></param>
        /// <param name="dashboardItemsOptions">options for DashboardResource Items property</param>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments, DashboardItemsOptions dashboardItemsOptions = DashboardItemsOptions.IncludeCurrentDeploymentOnly);
        Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments, CancellationToken cancellationToken, DashboardItemsOptions dashboardItemsOptions = DashboardItemsOptions.IncludeCurrentDeploymentOnly);
    }

    class DashboardRepository : IDashboardRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public DashboardRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<DashboardResource> GetDashboard()
        {
            return GetDashboard(CancellationToken.None);
        }

        public async Task<DashboardResource> GetDashboard(CancellationToken cancellationToken)
        {
            return await repository.Client.Get<DashboardResource>(await repository.Link("Dashboard").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments, DashboardItemsOptions dashboardItemsOptions = DashboardItemsOptions.IncludeCurrentDeploymentOnly)
        {
            return GetDynamicDashboard(projects, environments, CancellationToken.None, dashboardItemsOptions);
        }

        public async Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments, CancellationToken cancellationToken, DashboardItemsOptions dashboardItemsOptions = DashboardItemsOptions.IncludeCurrentDeploymentOnly)
        {
            var includePrevious = dashboardItemsOptions == DashboardItemsOptions.IncludeCurrentAndPreviousSuccessfulDeployment;
            return await repository.Client.Get<DashboardResource>(await repository.Link("DashboardDynamic").ConfigureAwait(false), new { projects, environments, includePrevious }, cancellationToken).ConfigureAwait(false);
        }
    }
}
