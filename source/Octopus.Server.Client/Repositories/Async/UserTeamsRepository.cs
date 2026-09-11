using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IUserTeamsRepository : ICanExtendSpaceContext<IUserTeamsRepository>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TeamNameResource[]> Get(UserResource user);
        Task<TeamNameResource[]> Get(UserResource user, CancellationToken cancellationToken);
    }

    class UserTeamsRepository : MixedScopeBaseRepository<TeamNameResource>, IUserTeamsRepository
    {
        public UserTeamsRepository(IOctopusAsyncRepository repository)
            : base(repository, null)
        {
        }

        UserTeamsRepository(IOctopusAsyncRepository repository, SpaceContext spaceContext)
            : base(repository, null, spaceContext)
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TeamNameResource[]> Get(UserResource user)
            => Get(user, CancellationToken.None);

        public async Task<TeamNameResource[]> Get(UserResource user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Client.Get<TeamNameResource[]>(user.Link("Teams"), GetAdditionalQueryParameters(), cancellationToken).ConfigureAwait(false);
        }

        public IUserTeamsRepository UsingContext(SpaceContext spaceContext)
        {
            return new UserTeamsRepository(Repository, spaceContext);
        }
    }
}
