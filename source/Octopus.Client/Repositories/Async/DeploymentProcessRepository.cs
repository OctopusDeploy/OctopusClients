using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentProcessRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel);
    }

    class DeploymentProcessRepository : BasicRepository<DeploymentProcessResource>, IDeploymentProcessRepository
    {
        public DeploymentProcessRepository(IOctopusAsyncRepository repository)
            : base(repository, "DeploymentProcesses")
        {
        }

        public Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id });
        }
    }
}
