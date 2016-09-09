using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDeploymentProcessRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel);
    }
}