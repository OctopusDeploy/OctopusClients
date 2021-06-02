using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IUserTeamsRepository : ICanExtendSpaceContext<IUserTeamsRepository>
    {
        Task<TeamNameResource[]> Get(UserResource user);
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

        public async Task<TeamNameResource[]> Get(UserResource user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Client.Get<TeamNameResource[]>(user.Link("Teams"), GetAdditionalQueryParameters()).ConfigureAwait(false);
        }

        public IUserTeamsRepository UsingContext(SpaceContext spaceContext)
        {
            return new UserTeamsRepository(Repository, spaceContext);
        }
    }
}