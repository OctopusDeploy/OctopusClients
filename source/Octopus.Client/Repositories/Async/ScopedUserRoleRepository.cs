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

        public IScopedUserRoleRepository LimitTo(bool includeGlobal, params string[] spaceIds)
        {
            var newParameters = this.CreateParameters(includeGlobal, spaceIds);
            return new ScopedUserRoleRepository(Client, newParameters);
        }
    }
}
