using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface IScopedUserRoleRepository :
        ICreate<ScopedUserRoleResource>,
        IModify<ScopedUserRoleResource>,
        IDelete<ScopedUserRoleResource>,
        IGet<ScopedUserRoleResource>,
        ICanExtendSpaceContext<IScopedUserRoleRepository>
    {
    }

    class ScopedUserRoleRepository : MixedScopeBaseRepository<ScopedUserRoleResource>, IScopedUserRoleRepository
    {
        public ScopedUserRoleRepository(IOctopusAsyncRepository repository)
            : base(repository, "ScopedUserRoles")
        {
            MinimumCompatibleVersion("2019.1.0");
        }

        ScopedUserRoleRepository(IOctopusAsyncRepository repository, SpaceContext spaceContext)
            : base(repository, "ScopedUserRoles", spaceContext)
        {
            MinimumCompatibleVersion("2019.1.0");
        }

        public IScopedUserRoleRepository UsingContext(SpaceContext spaceContext)
        {
            return new ScopedUserRoleRepository(Repository, spaceContext);
        }
    }
}
