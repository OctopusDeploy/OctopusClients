using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IOpsStepsRepository : IGet<OpsStepsResource>, IModify<OpsStepsResource>
    {
        OpsSnapshotTemplateResource GetTemplate(OpsStepsResource opsSteps);
    }
    
    class OpsStepsRepository : BasicRepository<OpsStepsResource>, IOpsStepsRepository
    {
        public OpsStepsRepository(IOctopusRepository repository)
            : base(repository, "OpsSteps")
        {
        }

        public OpsSnapshotTemplateResource GetTemplate(OpsStepsResource opsSteps)
        {
            return Client.Get<OpsSnapshotTemplateResource>(opsSteps.Link("OpsSnapshotTemplate"));
        }
    }
}