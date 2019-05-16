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
        private readonly IOctopusRepository repository;

        public DashboardRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public DashboardResource GetDashboard()
        {
            return repository.Client.Get<DashboardResource>(repository.Link("Dashboard"));
        }

        public DashboardResource GetDynamicDashboard(string[] projects, string[] environments)
        {
            return repository.Client.Get<DashboardResource>(repository.Link("DashboardDynamic"), new { projects, environments });
        }
    }
}