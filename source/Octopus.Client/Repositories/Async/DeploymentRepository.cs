using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentRepository : IGet<DeploymentResource>, ICreate<DeploymentResource>, IPaginate<DeploymentResource>
    {
        Task<TaskResource> GetTask(DeploymentResource resource);
        Task<ResourceCollection<DeploymentResource>> FindAll(string[] projects, string[] environments, int skip = 0);
        Task Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage);
        Task Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<DeploymentResource>, bool> getNextPage);
    }

    class DeploymentRepository : BasicRepository<DeploymentResource>, IDeploymentRepository
    {
        public DeploymentRepository(IOctopusAsyncClient client)
            : base(client, "Deployments")
        {
        }

        public Task<TaskResource> GetTask(DeploymentResource resource)
        {
            return Client.Get<TaskResource>(resource.Link("Task"));
        }

        public Task<ResourceCollection<DeploymentResource>> FindAll(string[] projects, string[] environments, int skip = 0)
        {
            return Client.List<DeploymentResource>(Client.RootDocument.Link("Deployments"), new { skip, projects = projects ?? new string[0], environments = environments ?? new string[0] });
        }

        public Task Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage)
        {
            return Client.Paginate(Client.RootDocument.Link("Deployments"), new { projects = projects ?? new string[0], environments = environments ?? new string[0] }, getNextPage);
        }

        public Task Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<DeploymentResource>, bool> getNextPage)
        {
            return Client.Paginate(Client.RootDocument.Link("Deployments"), new { projects = projects ?? new string[0], environments = environments ?? new string[0], tenants = tenants ?? new string[0] }, getNextPage);
        }
    }
}
