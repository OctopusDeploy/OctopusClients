using System.Linq;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    public interface IScopedUserRoleRepository :
        ICreate<ScopedUserRoleResource>,
        IModify<ScopedUserRoleResource>,
        IDelete<ScopedUserRoleResource>,
        IGet<ScopedUserRoleResource>,
        ICanIncludeSpaces<IScopedUserRoleRepository>
    {
    }
    
    class ScopedUserRoleRepository : MixedScopeBaseRepository<ScopedUserRoleResource>, IScopedUserRoleRepository
    {
        public ScopedUserRoleRepository(IOctopusClient client)
            : base(client, "ScopedUserRoles")
        {
        }

        ScopedUserRoleRepository(IOctopusClient client, SpaceQueryParameters spaceQueryParameters): base(client, "ScopedUserRoles")
        {
            SpaceQueryParameters = spaceQueryParameters;
        }

        public IScopedUserRoleRepository Including(bool includeGlobal, params string[] spaceIds)
        {
            return new ScopedUserRoleRepository(Client, new SpaceQueryParameters(includeGlobal, SpaceQueryParameters.SpaceIds.Concat(spaceIds).ToArray()));
        }

        public IScopedUserRoleRepository IncludingAllSpaces()
        {
            return new ScopedUserRoleRepository(Client, new SpaceQueryParameters(true, new[] { "all" }));
        }
    }
}