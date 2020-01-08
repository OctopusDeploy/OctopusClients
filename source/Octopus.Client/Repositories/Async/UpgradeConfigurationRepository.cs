using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IUpgradeConfigurationRepository : IGet<UpgradeConfigurationResource>, IModify<UpgradeConfigurationResource>
    {
        Task<UpgradeConfigurationResource> Get();
    }
    class UpgradeConfigurationRepository : BasicRepository<UpgradeConfigurationResource>, IUpgradeConfigurationRepository
    {
        public UpgradeConfigurationRepository(IOctopusAsyncRepository repository) : base(repository, "UpgradeConfiguration")
        {
        }

        public async Task<UpgradeConfigurationResource> Get()
        {
            var link = await ResolveLink();
            var upgradeConfiguration = await Client.Get<UpgradeConfigurationResource>(link);
            return upgradeConfiguration;
        }
    }
}