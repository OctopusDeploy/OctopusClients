using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDashboardRepository
    {
        Task<DashboardResource> GetDashboard();
        Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments);
    }
}