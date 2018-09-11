using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
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
        public ScopedUserRoleRepository(IOctopusClient client)
            : base(client, "ScopedUserRoles", null)
        {
        }

        ScopedUserRoleRepository(IOctopusClient client, SpaceQueryParameters spaceQueryParameters): base(client, "ScopedUserRoles", spaceQueryParameters)
        {
        }

        public IScopedUserRoleRepository LimitTo(bool includeSystem, params string[] spaceIds)
        {
            var newParameters = this.CreateParameters(includeSystem, spaceIds);
            return new ScopedUserRoleRepository(Client, newParameters);
        }
    }
}