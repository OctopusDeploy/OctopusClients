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
        private readonly IOctopusRepository repository;

        public DashboardConfigurationRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public DashboardConfigurationResource GetDashboardConfiguration()
        {
            return repository.Client.Get<DashboardConfigurationResource>(repository.Link("DashboardConfiguration"));
        }

        public DashboardConfigurationResource ModifyDashboardConfiguration(DashboardConfigurationResource resource)
        {
            return repository.Client.Update(repository.Link("DashboardConfiguration"), resource);
        }
    }
}