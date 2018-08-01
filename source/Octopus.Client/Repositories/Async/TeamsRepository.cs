using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface ITeamsRepository :
        ICreate<TeamResource>,
        IModify<TeamResource>,
        IDelete<TeamResource>,
        IFindByName<TeamResource>,
        IGet<TeamResource>,
        ICanLimitToSpaces<ITeamsRepository>
    {
        Task<List<ScopedUserRoleResource>> GetScopedUserRoles(TeamResource team);
    }

    class TeamsRepository : MixedScopeBaseRepository<TeamResource>, ITeamsRepository
    {
        public TeamsRepository(IOctopusAsyncClient client)
            : base(client, "Teams", null)
        {
        }

        TeamsRepository(IOctopusAsyncClient client, SpaceQueryParameters spaceQueryParameters)
            : base(client, "Teams", spaceQueryParameters)
        {
        }

        public async Task<List<ScopedUserRoleResource>> GetScopedUserRoles(TeamResource team)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));
            var resources = new List<ScopedUserRoleResource>();

            await Client.Paginate<ScopedUserRoleResource>(team.Link("ScopedUserRoles"), AdditionalQueryParameters, page =>
            {
                resources.AddRange(page.Items);
                return true;
            }).ConfigureAwait(false);

            return resources;
        }

        public ITeamsRepository LimitTo(bool includeGlobal, params string[] spaceIds)
        {
            var newParameters = this.CreateParameters(includeGlobal, spaceIds);
            return new TeamsRepository(Client, newParameters);
        }
    }
}
