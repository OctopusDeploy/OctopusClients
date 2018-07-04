using Octopus.Client.Model;

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
    
    class ScopedUserRoleRepository : BasicRepository<ScopedUserRoleResource>, IScopedUserRoleRepository
    {
        public ScopedUserRoleRepository(IOctopusClient client)
            : base(client, "ScopedUserRoles")
        {
        }

        public IScopedUserRoleRepository LimitTo(bool includeGlobal, params string[] spaceIds)
        {
            return new ScopedUserRoleRepository(Client)
            {
                LimitedToSpacesParameters = CreateSpacesParameters(includeGlobal, spaceIds)
            };
        }
    }
}