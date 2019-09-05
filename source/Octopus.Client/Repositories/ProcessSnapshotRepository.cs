using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IProcessSnapshotRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcessSnapshot, ChannelResource channel);
    }
    
    class ProcessSnapshotRepository : BasicRepository<DeploymentProcessResource>, IProcessSnapshotRepository
    {
        public ProcessSnapshotRepository(IOctopusRepository repository)
            : base(repository, "ProcessSnapshots")
        {
        }

        public ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id });
        }
    }
}