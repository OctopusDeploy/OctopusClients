using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDashboardConfigurationRepository
    {
        Task<DashboardConfigurationResource> GetDashboardConfiguration();
        Task<DashboardConfigurationResource> ModifyDashboardConfiguration(DashboardConfigurationResource resource);
    }
}