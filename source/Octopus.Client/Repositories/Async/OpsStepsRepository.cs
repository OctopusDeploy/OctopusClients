using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.OpsProcesses;

namespace Octopus.Client.Repositories.Async
{
    public interface IOpsStepsRepository : IGet<StepsResource>, IModify<StepsResource>
    {
        Task<OpsSnapshotTemplateResource> GetTemplate(StepsResource steps);
    }

    class OpsStepsRepository : BasicRepository<StepsResource>, IOpsStepsRepository
    {
        public OpsStepsRepository(IOctopusAsyncRepository repository)
            : base(repository, "OpsSteps")
        {
        }

        public Task<OpsSnapshotTemplateResource> GetTemplate(StepsResource steps)
        {
            return Client.Get<OpsSnapshotTemplateResource>(steps.Link("OpsSnapshotTemplate"));
        }
    }
}
