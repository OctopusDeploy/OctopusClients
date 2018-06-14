using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private static string ScopedUserRoleLink = "ScopedUserRole";
        public TeamsRepository(IOctopusAsyncClient client)
            : base(client, "Teams")
        {
        }

        public Task<ScopedUserRoleResource> CreateScopedUserRole(TeamResource team, ScopedUserRoleResource scopedUserRole)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));
            return Client.Post<object, ScopedUserRoleResource>(team.Link(ScopedUserRoleLink), scopedUserRole);
        }

        public Task UpdateScopedUserRole(TeamResource team, ScopedUserRoleResource scopedUserRole)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));
            return Client.Put(team.Link(ScopedUserRoleLink), scopedUserRole);
        }

        public async Task<List<ScopedUserRoleResource>> GetApiKeys(TeamResource team)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));
            var resources = new List<ScopedUserRoleResource>();

            await Client.Paginate<ScopedUserRoleResource>(team.Link(ScopedUserRoleLink), page =>
            {
                resources.AddRange(page.Items);
                return true;
            }).ConfigureAwait(false);

            return resources;
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
