using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IScopedUserRoleRepository :
        ICreate<ScopedUserRoleResource>,
        IModify<ScopedUserRoleResource>,
        IDelete<ScopedUserRoleResource>,
        IGet<ScopedUserRoleResource>
    {
    }
    
    class ScopedUserRoleRepository : MixScopeResourceRepository<ScopedUserRoleResource>, IScopedUserRoleRepository
    {
        public ScopedUserRoleRepository(IOctopusClient client)
            : base(client, "ScopedUserRoles")
        {
        }
    }
}