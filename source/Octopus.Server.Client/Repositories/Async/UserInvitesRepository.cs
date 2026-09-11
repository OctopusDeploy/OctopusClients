using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IUserInvitesRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<InvitationResource> Invite(string addToTeamId);
        Task<InvitationResource> Invite(string addToTeamId, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<InvitationResource> Invite(ReferenceCollection addToTeamIds);
        Task<InvitationResource> Invite(ReferenceCollection addToTeamIds, CancellationToken cancellationToken);
    }

    class UserInvitesRepository : MixedScopeBaseRepository<InvitationResource>, IUserInvitesRepository
    {
        public UserInvitesRepository(IOctopusAsyncRepository repository) : base(repository, "Invitations")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<InvitationResource> Invite(string addToTeamId)
            => Invite(addToTeamId, CancellationToken.None);

        public Task<InvitationResource> Invite(string addToTeamId, CancellationToken cancellationToken)
        {
            if (addToTeamId == null) throw new ArgumentNullException(nameof(addToTeamId));
            return Invite(new ReferenceCollection { addToTeamId }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<InvitationResource> Invite(ReferenceCollection addToTeamIds)
            => Invite(addToTeamIds, CancellationToken.None);

        public async Task<InvitationResource> Invite(ReferenceCollection addToTeamIds, CancellationToken cancellationToken)
        {
            var invitationResource = new InvitationResource { AddToTeamIds = addToTeamIds ?? new ReferenceCollection() };
            await EnrichSpaceId(invitationResource).ConfigureAwait(false);
            return await Create(invitationResource, cancellationToken).ConfigureAwait(false);
        }
    }
}
