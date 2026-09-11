using System;
using System.Collections.Generic;
using System.Threading;
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
        ICanExtendSpaceContext<ITeamsRepository>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<ScopedUserRoleResource>> GetScopedUserRoles(TeamResource team);
        Task<List<ScopedUserRoleResource>> GetScopedUserRoles(TeamResource team, CancellationToken cancellationToken);
    }

    class TeamsRepository : MixedScopeBaseRepository<TeamResource>, ITeamsRepository
    {
        public TeamsRepository(IOctopusAsyncRepository repository)
            : base(repository, "Teams")
        {
            MinimumCompatibleVersion("2019.1.0");
        }

        TeamsRepository(IOctopusAsyncRepository repository, SpaceContext spaceContext)
            : base(repository, "Teams", spaceContext)
        {
            MinimumCompatibleVersion("2019.1.0");
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<ScopedUserRoleResource>> GetScopedUserRoles(TeamResource team)
            => GetScopedUserRoles(team, CancellationToken.None);

        public async Task<List<ScopedUserRoleResource>> GetScopedUserRoles(TeamResource team, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            if (team == null) throw new ArgumentNullException(nameof(team));
            var resources = new List<ScopedUserRoleResource>();

            await Client.Paginate<ScopedUserRoleResource>(team.Link("ScopedUserRoles"), GetAdditionalQueryParameters(), page =>
            {
                resources.AddRange(page.Items);
                return true;
            }, cancellationToken).ConfigureAwait(false);

            return resources;
        }

        public ITeamsRepository UsingContext(SpaceContext spaceContext)
        {
            return new TeamsRepository(Repository, spaceContext);
        }
    }
}
