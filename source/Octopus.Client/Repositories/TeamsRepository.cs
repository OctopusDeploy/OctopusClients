using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITeamsRepository :
        ICreate<TeamResource>,
        IModify<TeamResource>,
        IDelete<TeamResource>,
        IFindByName<TeamResource>,
        IGet<TeamResource>
    {
        ScopedUserRoleResource CreateScopedUserRole(TeamResource team, ScopedUserRoleResource scopedUserRole);
        void UpdateScopedUserRole(TeamResource team, ScopedUserRoleResource scopedUserRole);
        List<ScopedUserRoleResource> GetScopedUserRoles(TeamResource team);
    }
    
    class TeamsRepository : BasicRepository<TeamResource>, ITeamsRepository
    {
        public TeamsRepository(IOctopusClient client)
            : base(client, "Teams")
        {
        }

        public ScopedUserRoleResource CreateScopedUserRole(TeamResource team, ScopedUserRoleResource scopedUserRole)
        {
            if (team == null) throw new ArgumentNullException("team");
            return Client.Post<object, ScopedUserRoleResource>(team.Link("ScopedUserRoles"), scopedUserRole);
        }

        public void UpdateScopedUserRole(TeamResource team, ScopedUserRoleResource scopedUserRole)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));
            Client.Put(team.Link("ScopedUserRoles"), scopedUserRole);
        }

        public List<ScopedUserRoleResource> GetScopedUserRoles(TeamResource team)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));
            var resources = new List<ScopedUserRoleResource>();

            Client.Paginate<ScopedUserRoleResource>(team.Link("ScopedUserRoles"), page =>
            {
                resources.AddRange(page.Items);
                return true;
            });

            return resources;
        }
    }
}