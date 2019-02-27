using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    // Used for backwards compatibility
    class LegacyInvitationRepository : BasicRepository<InvitationResource>, ICreate<InvitationResource>
    {
        public LegacyInvitationRepository(IOctopusAsyncRepository repository)
            : base(repository, "Invitations")
        {
        }
    }
}