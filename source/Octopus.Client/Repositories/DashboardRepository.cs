using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDashboardRepository
    {
        DashboardResource GetDashboard();

        /// <param name="projects"></param>
        /// <param name="environments"></param>
        /// <param name="dashboardItemsOptions">options for DashboardResource Items property</param>
        DashboardResource GetDynamicDashboard(string[] projects, string[] environments, DashboardItemsOptions dashboardItemsOptions = DashboardItemsOptions.IncludeCurrentDeploymentOnly);
    }

    class DashboardRepository : IDashboardRepository
    {
        private readonly IOctopusRepository repository;

        public DashboardRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public DashboardResource GetDashboard()
        {
            return repository.Client.Get<DashboardResource>(repository.Link("Dashboard"));
        }

        public DashboardResource GetDynamicDashboard(string[] projects, string[] environments, DashboardItemsOptions dashboardItemsOptions = DashboardItemsOptions.IncludeCurrentDeploymentOnly)
        {
            var includePrevious = dashboardItemsOptions == DashboardItemsOptions.IncludeCurrentAndPreviousSuccessfulDeployment;
            return repository.Client.Get<DashboardResource>(repository.Link("DashboardDynamic"), new { projects, environments, includePrevious });
        }
    }
}