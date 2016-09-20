using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDashboardConfigurationRepository
    {
        DashboardConfigurationResource GetDashboardConfiguration();
        DashboardConfigurationResource ModifyDashboardConfiguration(DashboardConfigurationResource resource);
    }
}