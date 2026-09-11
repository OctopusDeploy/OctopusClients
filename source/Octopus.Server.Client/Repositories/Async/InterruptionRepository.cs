using System;
using System.Threading;
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
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<InterruptionResource>> List(int skip = 0, int? take = null, bool pendingOnly = false, string regardingDocumentId = null);
        Task<ResourceCollection<InterruptionResource>> List(CancellationToken cancellationToken, int skip = 0, int? take = null, bool pendingOnly = false, string regardingDocumentId = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Submit(InterruptionResource interruption);
        Task Submit(InterruptionResource interruption, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task TakeResponsibility(InterruptionResource interruption);
        Task TakeResponsibility(InterruptionResource interruption, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<UserResource> GetResponsibleUser(InterruptionResource interruption);
        Task<UserResource> GetResponsibleUser(InterruptionResource interruption, CancellationToken cancellationToken);
    }

    class InterruptionRepository : BasicRepository<InterruptionResource>, IInterruptionRepository
    {
        public InterruptionRepository(IOctopusAsyncRepository repository)
            : base(repository, "Interruptions")
        {
        }

        public Task<ResourceCollection<InterruptionResource>> List(int skip = 0, int? take = null, bool pendingOnly = false, string regardingDocumentId = null)
        {
            return List(CancellationToken.None, skip, take, pendingOnly, regardingDocumentId);
        }

        public async Task<ResourceCollection<InterruptionResource>> List(CancellationToken cancellationToken, int skip = 0, int? take = null, bool pendingOnly = false, string regardingDocumentId = null)
        {
            return await Client.List<InterruptionResource>(await Repository.Link("Interruptions").ConfigureAwait(false), new { skip, take, pendingOnly, regarding = regardingDocumentId }, cancellationToken).ConfigureAwait(false);
        }

        public Task Submit(InterruptionResource interruption)
        {
            return Submit(interruption, CancellationToken.None);
        }

        public Task Submit(InterruptionResource interruption, CancellationToken cancellationToken)
        {
            return Client.Post(interruption.Link("Submit"), interruption.Form.Values, cancellationToken);
        }

        public Task TakeResponsibility(InterruptionResource interruption)
        {
            return TakeResponsibility(interruption, CancellationToken.None);
        }

        public Task TakeResponsibility(InterruptionResource interruption, CancellationToken cancellationToken)
        {
            return Client.Put(interruption.Link("Responsible"), (InterruptionResource)null, cancellationToken);
        }

        public Task<UserResource> GetResponsibleUser(InterruptionResource interruption)
        {
            return GetResponsibleUser(interruption, CancellationToken.None);
        }

        public Task<UserResource> GetResponsibleUser(InterruptionResource interruption, CancellationToken cancellationToken)
        {
            return Client.Get<UserResource>(interruption.Link("Responsible"), cancellationToken);
        }
    }
}
