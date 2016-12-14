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
    
    class DeploymentRepository : BasicRepository<DeploymentResource>, IDeploymentRepository
    {
        public DeploymentRepository(IOctopusClient client)
            : base(client, "Deployments")
        {
        }

        public TaskResource GetTask(DeploymentResource resource)
        {
            return Client.Get<TaskResource>(resource.Link("Task"));
        }

        public ResourceCollection<DeploymentResource> FindAll(string[] projects, string[] environments, int skip = 0)
        {
            return Client.List<DeploymentResource>(Client.RootDocument.Link("Deployments"), new { skip, projects = projects ?? new string[0], environments = environments ?? new string[0] });
        }

        public void Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage)
        {
            Client.Paginate(Client.RootDocument.Link("Deployments"), new { projects = projects ?? new string[0], environments = environments ?? new string[0] }, getNextPage);
        }
    }
}