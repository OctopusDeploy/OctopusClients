using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.OpsProcesses;

namespace Octopus.Client.Repositories.Async
{
    public interface IOpsStepsRepository : IGet<OpsStepsResource>, IModify<OpsStepsResource>
    {
        Task<OpsSnapshotTemplateResource> GetTemplate(OpsStepsResource opsSteps);
    }

    class OpsStepsRepository : BasicRepository<OpsStepsResource>, IOpsStepsRepository
    {
        public OpsStepsRepository(IOctopusAsyncRepository repository)
            : base(repository, "OpsSteps")
        {
        }

        public Task<OpsSnapshotTemplateResource> GetTemplate(OpsStepsResource opsSteps)
        {
            return Client.Get<OpsSnapshotTemplateResource>(opsSteps.Link("OpsSnapshotTemplate"));
        }
    }
}
