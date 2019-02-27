using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{ 
    public interface IUserInvitesRepository
    {
        Task<InvitationResource> Invite(string addToTeamId);
        Task<InvitationResource> Invite(ReferenceCollection addToTeamIds);
    }
    
    class UserInvitesRepository : MixedScopeBaseRepository<InvitationResource>, IUserInvitesRepository
    {
        public UserInvitesRepository(IOctopusAsyncRepository repository) : base(repository, "Invitations")
        {
        }

        public Task<InvitationResource> Invite(string addToTeamId)
        {
            if (addToTeamId == null) throw new ArgumentNullException(nameof(addToTeamId));
            return Invite(new ReferenceCollection { addToTeamId });
        }

        public Task<InvitationResource> Invite(ReferenceCollection addToTeamIds)
        {
            var invitationResource = new InvitationResource { AddToTeamIds = addToTeamIds ?? new ReferenceCollection() };
            EnrichSpaceId(invitationResource);
            return Create(invitationResource);
        }
    }
}