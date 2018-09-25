using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    class InvitationRepository : BasicRepository<InvitationResource>, ICreate<InvitationResource>
    {
        public InvitationRepository(IOctopusAsyncRepository repository)
            : base(repository, "Invitations")
        {
        }
    }
}