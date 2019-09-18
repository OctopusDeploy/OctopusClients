using Octopus.Client.Model;
using Octopus.Client.Model.OpsProcesses;

namespace Octopus.Client.Repositories
{
    public interface IOpsStepsRepository : IGet<StepsResource>, IModify<StepsResource>
    {
        OpsSnapshotTemplateResource GetTemplate(StepsResource deploymentSteps);
    }
    
    class OpsStepsRepository : BasicRepository<StepsResource>, IOpsStepsRepository
    {
        public OpsStepsRepository(IOctopusRepository repository)
            : base(repository, "OpsSteps")
        {
        }

        public OpsSnapshotTemplateResource GetTemplate(StepsResource deploymentProcess)
        {
            return Client.Get<OpsSnapshotTemplateResource>(deploymentProcess.Link("OpsSnapshotTemplate"));
        }
    }
}