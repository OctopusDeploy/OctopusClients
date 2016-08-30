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
}