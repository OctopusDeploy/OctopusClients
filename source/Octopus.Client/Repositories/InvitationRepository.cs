using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    class InvitationRepository : BasicRepository<InvitationResource>, ICreate<InvitationResource>
    {
        public InvitationRepository(IOctopusRepository repository)
            : base(repository, "Invitations")
        {
        }
    }
}