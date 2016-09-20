using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDashboardRepository
    {
        DashboardResource GetDashboard();
        DashboardResource GetDynamicDashboard(string[] projects, string[] environments);
    }
}