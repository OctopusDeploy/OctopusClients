using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IInterruptionRepository : IGet<InterruptionResource>
    {
        Task<ResourceCollection<InterruptionResource>> List(int skip = 0, bool pendingOnly = false, string regardingDocumentId = null);
        Task Submit(InterruptionResource interruption);
        Task TakeResponsibility(InterruptionResource interruption);
        Task<UserResource> GetResponsibleUser(InterruptionResource interruption);
    }
}