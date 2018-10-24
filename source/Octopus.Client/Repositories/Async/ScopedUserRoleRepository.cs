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
        }

        ScopedUserRoleRepository(IOctopusAsyncRepository repository, SpaceContext includingSpaceContext, SpaceContext extendedSpaceContext)
            : base(repository, "ScopedUserRoles", includingSpaceContext, extendedSpaceContext)
        {
        }

        public IScopedUserRoleRepository UsingContext(SpaceContext spaceContext)
        {
            return new ScopedUserRoleRepository(Repository, spaceContext, this.GetCurrentSpaceContext());
        }
    }
}
