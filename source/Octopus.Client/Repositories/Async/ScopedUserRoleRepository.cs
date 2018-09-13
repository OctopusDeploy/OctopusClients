using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface IScopedUserRoleRepository :
        ICreate<ScopedUserRoleResource>,
        IModify<ScopedUserRoleResource>,
        IDelete<ScopedUserRoleResource>,
        IGet<ScopedUserRoleResource>,
        ICanLimitToSpaces<IScopedUserRoleRepository>
    {
    }

    class ScopedUserRoleRepository : MixedScopeBaseRepository<ScopedUserRoleResource>, IScopedUserRoleRepository
    {
        public ScopedUserRoleRepository(IOctopusAsyncClient client)
            : base(client, "ScopedUserRoles", null)
        {
        }

        ScopedUserRoleRepository(IOctopusAsyncClient client, SpaceQueryContext spaceQueryContext)
            : base(client, "ScopedUserRoles", spaceQueryContext)
        {
        }

        public IScopedUserRoleRepository LimitTo(bool includeSystem, params string[] spaceIds)
        {
            return new ScopedUserRoleRepository(Client, CreateSpaceQueryContext(includeSystem, spaceIds));
        }
    }
}
