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
        public ScopedUserRoleRepository(IOctopusRepository repository)
            : base(repository, "ScopedUserRoles")
        {
        }

        ScopedUserRoleRepository(IOctopusRepository repository, SpaceContext includingSpaceContext, SpaceContext extendedSpaceContext)
            : base(repository, "ScopedUserRoles", includingSpaceContext, extendedSpaceContext)
        {
        }

        public IScopedUserRoleRepository Including(SpaceContext spaceContext)
        {
            return new ScopedUserRoleRepository(Repository, spaceContext, GetCurrentSpaceContext());
        }
    }
}