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
        }

        ScopedUserRoleRepository(IOctopusAsyncRepository repository, SpaceContext spaceContext)
            : base(repository, "ScopedUserRoles", spaceContext)
        {
        }

        public IScopedUserRoleRepository Including(SpaceContext spaceContext)
        {
            return new ScopedUserRoleRepository(Repository, ExtendSpaceContext(spaceContext));
        }
    }
}
