using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDashboardRepository
    {
        DashboardResource GetDashboard();
        
        /// <param name="projects"></param>
        /// <param name="environments"></param>
        /// <param name="withLatestSuccessfulRelease">includes latest successful release in the list of Items</param>
        DashboardResource GetDynamicDashboard(string[] projects, string[] environments, bool withLatestSuccessfulRelease = false);
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

        public DashboardResource GetDynamicDashboard(string[] projects, string[] environments, bool withLatestSuccessfulRelease = false)
        {
            var includePrevious = withLatestSuccessfulRelease;
            return repository.Client.Get<DashboardResource>(repository.Link("DashboardDynamic"), new { projects, environments, includePrevious });
        }
    }
}