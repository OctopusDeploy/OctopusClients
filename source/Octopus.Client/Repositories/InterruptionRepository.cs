using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IInterruptionRepository : IGet<InterruptionResource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <param name="pendingOnly"></param>
        /// <param name="regardingDocumentId"></param>
        /// <returns></returns>
        ResourceCollection<InterruptionResource> List(int skip = 0, int? take = null, bool pendingOnly = false, string regardingDocumentId = null);
        void Submit(InterruptionResource interruption);
        void TakeResponsibility(InterruptionResource interruption);
        UserResource GetResponsibleUser(InterruptionResource interruption);
    }
    
    class InterruptionRepository : BasicRepository<InterruptionResource>, IInterruptionRepository
    {
        public InterruptionRepository(IOctopusRepository repository)
            : base(repository, "Interruptions")
        {
        }

        public ResourceCollection<InterruptionResource> List(int skip = 0, int? take = null, bool pendingOnly = false, string regardingDocumentId = null)
        {
            return Client.List<InterruptionResource>(Repository.Link("Interruptions"), new { skip,take, pendingOnly, regarding = regardingDocumentId });
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