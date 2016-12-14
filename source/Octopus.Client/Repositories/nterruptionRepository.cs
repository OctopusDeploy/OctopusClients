using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IInterruptionRepository : IGet<InterruptionResource>
    {
        ResourceCollection<InterruptionResource> List(int skip = 0, bool pendingOnly = false, string regardingDocumentId = null);
        void Submit(InterruptionResource interruption);
        void TakeResponsibility(InterruptionResource interruption);
        UserResource GetResponsibleUser(InterruptionResource interruption);
    }
    
    class InterruptionRepository : BasicRepository<InterruptionResource>, IInterruptionRepository
    {
        public InterruptionRepository(IOctopusClient client)
            : base(client, "Interruptions")
        {
        }

        public ResourceCollection<InterruptionResource> List(int skip = 0, bool pendingOnly = false, string regardingDocumentId = null)
        {
            return Client.List<InterruptionResource>(Client.RootDocument.Link("Interruptions"), new { skip, pendingOnly, regarding = regardingDocumentId });
        }

        public void Submit(InterruptionResource interruption)
        {
            Client.Post(interruption.Link("Submit"), interruption.Form.Values);
        }

        public void TakeResponsibility(InterruptionResource interruption)
        {
            Client.Put(interruption.Link("Responsible"), (InterruptionResource)null);
        }

        public UserResource GetResponsibleUser(InterruptionResource interruption)
        {
            return Client.Get<UserResource>(interruption.Link("Responsible"));
        }
    }
}