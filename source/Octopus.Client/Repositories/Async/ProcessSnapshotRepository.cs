using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IProcessSnapshotRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel);
    }

    class ProcessSnapshotRepository : BasicRepository<DeploymentProcessResource>, IProcessSnapshotRepository
    {
        public ProcessSnapshotRepository(IOctopusAsyncRepository repository)
            : base(repository, "ProcessSnapshots")
        {
        }

        public Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id });
        }
    }
}
