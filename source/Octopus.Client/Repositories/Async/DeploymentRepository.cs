using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentRepository : IGet<DeploymentResource>, ICreate<DeploymentResource>, IPaginate<DeploymentResource>
    {
        Task<TaskResource> GetTask(DeploymentResource resource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="environments"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        Task<ResourceCollection<DeploymentResource>> FindBy(string[] projects, string[] environments, int skip = 0, int? take = null);

        [Obsolete("This method is not a find all, it still requires paging. So it has been renamed to `FindBy`")]
        Task<ResourceCollection<DeploymentResource>> FindAll(string[] projects, string[] environments, int skip = 0, int? take = null);
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

        public Task<ResourceCollection<DeploymentResource>> FindBy(string[] projects, string[] environments, int skip = 0, int? take = null)
        {
            return Client.List<DeploymentResource>(Client.RootDocument.Link("Deployments"), new { skip, take, projects = projects ?? new string[0], environments = environments ?? new string[0] });
        }

        [Obsolete("This method is not a find all, it still requires paging. So it has been renamed to `FindBy`")]
        public Task<ResourceCollection<DeploymentResource>> FindAll(string[] projects, string[] environments, int skip = 0, int? take = null)
        {
            return FindBy(projects, environments, skip, take);
        }

        public Task Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage)
        {
            return Paginate(projects, environments, new string[0], getNextPage);
        }

        public Task Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<DeploymentResource>, bool> getNextPage)
        {
            return Client.Paginate(Client.RootDocument.Link("Deployments"), new { projects = projects ?? new string[0], environments = environments ?? new string[0], tenants = tenants ?? new string[0] }, getNextPage);
        }
    }
}
