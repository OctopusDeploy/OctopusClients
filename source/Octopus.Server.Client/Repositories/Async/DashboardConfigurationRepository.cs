using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDashboardConfigurationRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DashboardConfigurationResource> GetDashboardConfiguration();
        Task<DashboardConfigurationResource> GetDashboardConfiguration(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DashboardConfigurationResource> ModifyDashboardConfiguration(DashboardConfigurationResource resource);
        Task<DashboardConfigurationResource> ModifyDashboardConfiguration(DashboardConfigurationResource resource, CancellationToken cancellationToken);
    }

    class DashboardConfigurationRepository : IDashboardConfigurationRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public DashboardConfigurationRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<DashboardConfigurationResource> GetDashboardConfiguration()
        {
            return GetDashboardConfiguration(CancellationToken.None);
        }

        public async Task<DashboardConfigurationResource> GetDashboardConfiguration(CancellationToken cancellationToken)
        {
            return await repository.Client.Get<DashboardConfigurationResource>(await repository.Link("DashboardConfiguration").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<DashboardConfigurationResource> ModifyDashboardConfiguration(DashboardConfigurationResource resource)
        {
            return ModifyDashboardConfiguration(resource, CancellationToken.None);
        }

        public async Task<DashboardConfigurationResource> ModifyDashboardConfiguration(DashboardConfigurationResource resource, CancellationToken cancellationToken)
        {
            return await repository.Client.Update(await repository.Link("DashboardConfiguration").ConfigureAwait(false), resource, cancellationToken).ConfigureAwait(false);
        }
    }
}
