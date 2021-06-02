using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IUserTeamsRepository :
        ICanExtendSpaceContext<IUserTeamsRepository>
    {
        TeamNameResource[] Get(UserResource user);
    }

    class UserTeamsRepository : MixedScopeBaseRepository<TeamNameResource>, IUserTeamsRepository
    {
        public UserTeamsRepository(IOctopusRepository repository)
            : base(repository, null)
        {
        }

        UserTeamsRepository(IOctopusRepository repository, SpaceContext userDefinedSpaceContext)
            : base(repository, null, userDefinedSpaceContext)
        {
        }

        public TeamNameResource[] Get(UserResource user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Client.Get<TeamNameResource[]>(user.Link("Teams"), AdditionalQueryParameters);
        }

        public IUserTeamsRepository UsingContext(SpaceContext userDefinedSpaceContext)
        {
            return new UserTeamsRepository(Repository, userDefinedSpaceContext);
        }
    }
}