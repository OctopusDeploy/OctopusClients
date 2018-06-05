using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ITeamsRepository :
        ICreate<TeamResource>,
        IModify<TeamResource>,
        IDelete<TeamResource>,
        IFindByName<TeamResource>,
        IGet<TeamResource>
    {
    }

    class TeamsRepository : BasicRepository<TeamResource>, ITeamsRepository
    {
        public TeamsRepository(IOctopusAsyncClient client)
            : base(client, "Teams")
        {
        }
    }

    public interface IScopedUserRolesRepository :
        ICreate<ScopedUserRoleResource>,
        IModify<ScopedUserRoleResource>,
        IDelete<ScopedUserRoleResource>,
        IGet<ScopedUserRoleResource>,
        IPaginate<ScopedUserRoleResource>
    {
    }

    class ScopedUserRolesRepository : BasicRepository<ScopedUserRoleResource>, IScopedUserRolesRepository
    {
        public ScopedUserRolesRepository(IOctopusAsyncClient client)
            : base(client, "ScopedUserRoles")
        {
        }
    }
}
