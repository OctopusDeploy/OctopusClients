using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    public interface ITeamsRepository :
        ICreate<TeamResource>,
        IModify<TeamResource>,
        IDelete<TeamResource>,
        IFindByName<TeamResource>,
        IGet<TeamResource>,
        ICanExpandSpaceContext<ITeamsRepository>
    {
        List<ScopedUserRoleResource> GetScopedUserRoles(TeamResource team);
    }
    
    class TeamsRepository : MixedScopeBaseRepository<TeamResource>, ITeamsRepository
    {
        public TeamsRepository(IOctopusClient client)
            : base(client, "Teams")
        {
        }

        TeamsRepository(IOctopusClient client, SpaceQueryParameters spaceQueryParameters): base(client, "Teams")
        {
            SpaceQueryParameters = spaceQueryParameters;
        }

        public List<ScopedUserRoleResource> GetScopedUserRoles(TeamResource team)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));
            var resources = new List<ScopedUserRoleResource>();

            Client.Paginate<ScopedUserRoleResource>(team.Link("ScopedUserRoles"), AdditionalQueryParameters, page =>
            {
                resources.AddRange(page.Items);
                return true;
            });

            return resources;
        }

        public ITeamsRepository Including(bool includeGlobal, params string[] spaceIds)
        {
            return new TeamsRepository(Client, new SpaceQueryParameters(includeGlobal, SpaceQueryParameters.SpaceIds.Concat(spaceIds).ToArray()));
        }
    }
}