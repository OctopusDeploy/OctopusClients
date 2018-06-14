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
        List<ScopedUserRoleResource> GetApiKeys(TeamResource team);
    }
    
    class TeamsRepository : BasicRepository<TeamResource>, ITeamsRepository
    {
        private static string ScopedUserRoleLink = "ScopedUserRoles";
        public TeamsRepository(IOctopusClient client)
            : base(client, "Teams")
        {
        }

        public ScopedUserRoleResource CreateScopedUserRole(TeamResource team, ScopedUserRoleResource scopedUserRole)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));
            return Client.Post<object, ScopedUserRoleResource>(team.Link(ScopedUserRoleLink), scopedUserRole);
        }

        public void UpdateScopedUserRole(TeamResource team, ScopedUserRoleResource scopedUserRole)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));
            Client.Put(team.Link(ScopedUserRoleLink), scopedUserRole);
        }

        public List<ScopedUserRoleResource> GetApiKeys(TeamResource team)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));
            var resources = new List<ScopedUserRoleResource>();

            Client.Paginate<ScopedUserRoleResource>(team.Link(ScopedUserRoleLink), page =>
            {
                resources.AddRange(page.Items);
                return true;
            });

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
        public ScopedUserRolesRepository(IOctopusClient client)
            : base(client, "ScopedUserRoles")
        {
        }
    }
}