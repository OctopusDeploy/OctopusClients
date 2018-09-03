using System;
using System.Collections.Generic;
using System.Linq;
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
        ICanIncludeSpaces<ITeamsRepository>
    {
        Task<List<ScopedUserRoleResource>> GetScopedUserRoles(TeamResource team);
    }

    class TeamsRepository : MixedScopeBaseRepository<TeamResource>, ITeamsRepository
    {
        public TeamsRepository(IOctopusAsyncClient client)
            : base(client, "Teams")
        {
        }

        TeamsRepository(IOctopusAsyncClient client, SpaceQueryParameters spaceQueryParameters): base(client, "Teams")
        {
            SpaceQueryParameters = spaceQueryParameters;
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

        public ITeamsRepository Including(bool includeGlobal, params string[] spaceIds)
        {
            return new TeamsRepository(Client, new SpaceQueryParameters(includeGlobal, SpaceQueryParameters.SpaceIds.Concat(spaceIds).ToArray()));
        }

        public ITeamsRepository IncludingAllSpaces()
        {
            return new TeamsRepository(Client, new SpaceQueryParameters(true, new []{"all"}));
        }
    }
}
