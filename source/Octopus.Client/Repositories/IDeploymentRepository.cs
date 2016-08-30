using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDeploymentRepository : IGet<DeploymentResource>, ICreate<DeploymentResource>, IPaginate<DeploymentResource>
    {
        TaskResource GetTask(DeploymentResource resource);
        ResourceCollection<DeploymentResource> FindAll(string[] projects, string[] environments, int skip = 0);
        void Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage);
    }
}