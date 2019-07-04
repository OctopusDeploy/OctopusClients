using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IUpgradeConfigurationRepository : IGet<UpgradeConfigurationResource>, IModify<UpgradeConfigurationResource>
    {
        UpgradeConfigurationResource Get();
    }

    class UpgradeConfigurationRepository : BasicRepository<UpgradeConfigurationResource>, IModify<UpgradeConfigurationResource>, IUpgradeConfigurationRepository
    {
        public UpgradeConfigurationRepository(IOctopusRepository repository) : base(repository, "UpgradeConfiguration")
        {
        }

        public UpgradeConfigurationResource Get()
        {
            var link = ResolveLink();
            var upgradeConfiguration = Client.Get<UpgradeConfigurationResource>(link);
            return upgradeConfiguration;
        }
    }
}