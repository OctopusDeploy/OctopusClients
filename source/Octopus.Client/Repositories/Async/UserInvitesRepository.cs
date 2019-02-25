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
        readonly BasicRepository<InvitationResource> invitations;

        public UserInvitesRepository(IOctopusAsyncRepository repository) : base(repository, "Invitations")
        {
            invitations = new InvitationRepository(Repository);
        }

        public async Task<InvitationResource> Invite(string addToTeamId)
        {
            if (addToTeamId == null) throw new ArgumentNullException(nameof(addToTeamId));
            return await Invite(new ReferenceCollection { addToTeamId });
        }

        public async Task<InvitationResource> Invite(ReferenceCollection addToTeamIds)
        {
            var invitationResource = new InvitationResource { AddToTeamIds = addToTeamIds ?? new ReferenceCollection() };
            EnrichSpaceId(invitationResource);
            return await invitations.Create(invitationResource);
        }
    }
}