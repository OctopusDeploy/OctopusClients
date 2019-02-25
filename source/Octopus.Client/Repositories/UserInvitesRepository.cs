using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IUserInvitesRepository
    {
        InvitationResource Invite(string addToTeamId);
        InvitationResource Invite(ReferenceCollection addToTeamIds);
    }
    
    class UserInvitesRepository : MixedScopeBaseRepository<InvitationResource>, IUserInvitesRepository
    {
        readonly BasicRepository<InvitationResource> invitations;

        public UserInvitesRepository(IOctopusRepository octopusRepository) : base(octopusRepository, "Invitations")
        {
            invitations = new InvitationRepository(octopusRepository);
        }
        
        public InvitationResource Invite(string addToTeamId)
        {
            if (addToTeamId == null) throw new ArgumentNullException(nameof(addToTeamId));
            return Invite(new ReferenceCollection { addToTeamId });
        }

        public InvitationResource Invite(ReferenceCollection addToTeamIds)
        {
            var invitationResource = new InvitationResource { AddToTeamIds = addToTeamIds ?? new ReferenceCollection() };
            EnrichSpaceId(invitationResource);
            return invitations.Create(invitationResource);
        }
    }
}