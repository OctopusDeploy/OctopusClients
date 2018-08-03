using System;
using System.Collections.Generic;
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
        ICanLimitToSpaces<ITeamsRepository>
    {
        List<ScopedUserRoleResource> GetScopedUserRoles(TeamResource team);
    }
    
    class TeamsRepository : MixedScopeBaseRepository<TeamResource>, ITeamsRepository
    {
        public TeamsRepository(IOctopusClient client)
            : base(client, "Teams", null)
        {
        }

        TeamsRepository(IOctopusClient client, SpaceQueryParameters spaceQueryParameters)
            : base(client, "Teams", spaceQueryParameters)
        {
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

        public ITeamsRepository LimitTo(bool includeGlobal, params string[] spaceIds)
        {
            var newParameters = this.CreateParameters(includeGlobal, spaceIds);
            return new TeamsRepository(Client, newParameters);
        }
    }
}