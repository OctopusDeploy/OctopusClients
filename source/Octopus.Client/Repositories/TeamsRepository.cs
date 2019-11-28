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
        ICanExtendSpaceContext<ITeamsRepository>
    {
        List<ScopedUserRoleResource> GetScopedUserRoles(TeamResource team);
    }
    
    class TeamsRepository : MixedScopeBaseRepository<TeamResource>, ITeamsRepository
    {
        public TeamsRepository(IOctopusRepository repository)
            : base(repository, "Teams")
        {
            //MinimumCompatibleVersion("2019.1.0");
        }

        TeamsRepository(IOctopusRepository repository, SpaceContext userDefinedSpaceContext)
            : base(repository, "Teams", userDefinedSpaceContext)
        {
            //MinimumCompatibleVersion("2019.1.0");
        }

        public List<ScopedUserRoleResource> GetScopedUserRoles(TeamResource team)
        {
            ThrowIfServerVersionIsNotCompatible();
            
            if (team == null) throw new ArgumentNullException(nameof(team));
            var resources = new List<ScopedUserRoleResource>();

            Client.Paginate<ScopedUserRoleResource>(team.Link("ScopedUserRoles"), AdditionalQueryParameters, page =>
            {
                resources.AddRange(page.Items);
                return true;
            });

            return resources;
        }

        public ITeamsRepository UsingContext(SpaceContext userDefinedSpaceContext)
        {
            ThrowIfServerVersionIsNotCompatible();
            
            return new TeamsRepository(Repository, userDefinedSpaceContext);
        }
    }
}