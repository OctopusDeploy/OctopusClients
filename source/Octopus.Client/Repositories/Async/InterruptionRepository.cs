using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IInterruptionRepository : IGet<InterruptionResource>
    {
        Task<ResourceCollection<InterruptionResource>> List(int skip = 0, bool pendingOnly = false, string regardingDocumentId = null);
        Task Submit(InterruptionResource interruption);
        Task TakeResponsibility(InterruptionResource interruption);
        Task<UserResource> GetResponsibleUser(InterruptionResource interruption);
    }

    class InterruptionRepository : BasicRepository<InterruptionResource>, IInterruptionRepository
    {
        public InterruptionRepository(IOctopusAsyncClient client)
            : base(client, "Interruptions")
        {
        }

        public Task<ResourceCollection<InterruptionResource>> List(int skip = 0, bool pendingOnly = false, string regardingDocumentId = null)
        {
            return Client.List<InterruptionResource>(Client.RootDocument.Link("Interruptions"), new { skip, pendingOnly, regarding = regardingDocumentId });
        }

        public Task Submit(InterruptionResource interruption)
        {
            return Client.Post(interruption.Link("Submit"), interruption.Form.Values);
        }

        public Task TakeResponsibility(InterruptionResource interruption)
        {
            return Client.Put(interruption.Link("Responsible"), (InterruptionResource)null);
        }

        public Task<UserResource> GetResponsibleUser(InterruptionResource interruption)
        {
            return Client.Get<UserResource>(interruption.Link("Responsible"));
        }
    }
}
