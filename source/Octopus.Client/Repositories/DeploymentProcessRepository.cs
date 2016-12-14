using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDeploymentProcessRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel);
    }
    
    class DeploymentProcessRepository : BasicRepository<DeploymentProcessResource>, IDeploymentProcessRepository
    {
        public DeploymentProcessRepository(IOctopusClient client)
            : base(client, "DeploymentProcesses")
        {
        }

        public ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id });
        }
    }
}