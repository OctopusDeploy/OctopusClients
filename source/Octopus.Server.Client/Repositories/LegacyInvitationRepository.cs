using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    // Used for backwards compatibility
    class LegacyInvitationRepository : BasicRepository<InvitationResource>, ICreate<InvitationResource>
    {
        public LegacyInvitationRepository(IOctopusRepository repository)
            : base(repository, "Invitations")
        {
        }
    }
}