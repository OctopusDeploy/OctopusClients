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
        ICanExtendSpaceContext<IScopedUserRoleRepository>
    {
    }
    
    class ScopedUserRoleRepository : MixedScopeBaseRepository<ScopedUserRoleResource>, IScopedUserRoleRepository
    {
        public ScopedUserRoleRepository(IOctopusClient client)
            : base(client, "ScopedUserRoles")
        {
        }

        ScopedUserRoleRepository(IOctopusClient client, SpaceContext spaceContext)
            : base(client, "ScopedUserRoles", spaceContext)
        {
        }

        public IScopedUserRoleRepository Including(SpaceContext spaceContext)
        {
            return new ScopedUserRoleRepository(Client, ExtendSpaceContext(spaceContext));
        }
    }
}