using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IUpgradeConfigurationRepository : IGet<UpgradeConfigurationResource>, IModify<UpgradeConfigurationResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<UpgradeConfigurationResource> Get();
        Task<UpgradeConfigurationResource> Get(CancellationToken cancellationToken);
    }
    class UpgradeConfigurationRepository : BasicRepository<UpgradeConfigurationResource>, IUpgradeConfigurationRepository
    {
        public UpgradeConfigurationRepository(IOctopusAsyncRepository repository) : base(repository, "UpgradeConfiguration")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<UpgradeConfigurationResource> Get()
            => Get(CancellationToken.None);

        public async Task<UpgradeConfigurationResource> Get(CancellationToken cancellationToken)
        {
            var link = await ResolveLink(cancellationToken);
            var upgradeConfiguration = await Client.Get<UpgradeConfigurationResource>(link, cancellationToken);
            return upgradeConfiguration;
        }
    }
}
