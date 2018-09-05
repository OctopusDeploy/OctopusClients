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

        public ScopedUserRoleRepository(IOctopusClient client, SpaceQueryParameters spaceQueryParameters)
            : base(client, "ScopedUserRoles")
        {
            SpaceQueryParameters = spaceQueryParameters;
        }

        public IScopedUserRoleRepository Including(SpaceContext spaceContext)
        {
            return new ScopedUserRoleRepository(Client, Client.SpaceContext.Union(spaceContext).ToSpaceQueryParameters());
        }
    }
}