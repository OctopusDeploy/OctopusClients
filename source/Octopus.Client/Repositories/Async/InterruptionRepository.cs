using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IInterruptionRepository : IGet<InterruptionResource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <param name="pendingOnly"></param>
        /// <param name="regardingDocumentId"></param>
        /// <returns></returns>
        Task<ResourceCollection<InterruptionResource>> List(int skip = 0, int? take = null, bool pendingOnly = false, string regardingDocumentId = null);
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

        public Task<ResourceCollection<InterruptionResource>> List(int skip = 0, int? take = null, bool pendingOnly = false, string regardingDocumentId = null)
        {
            return Client.List<InterruptionResource>(Client.RootDocument.Link("Interruptions"), new { skip, take, pendingOnly, regarding = regardingDocumentId });
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
