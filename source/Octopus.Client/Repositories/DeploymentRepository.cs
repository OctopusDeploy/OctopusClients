using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDeploymentRepository : IGet<DeploymentResource>, ICreate<DeploymentResource>, IPaginate<DeploymentResource>, IDelete<DeploymentResource>
    {
        TaskResource GetTask(DeploymentResource resource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="environments"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <returns></returns>
        ResourceCollection<DeploymentResource> FindBy(string[] projects, string[] environments, int skip = 0, int? take = null);

        [Obsolete("This method is not a find all, it still requires paging. So it has been renamed to `FindBy`")]
        ResourceCollection<DeploymentResource> FindAll(string[] projects, string[] environments, int skip = 0, int? take = null);
        void Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage);
        void Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<DeploymentResource>, bool> getNextPage);

    }

    class DeploymentRepository : BasicRepository<DeploymentResource>, IDeploymentRepository
    {
        public DeploymentRepository(IOctopusRepository repository)
            : base(repository, "Deployments")
        {
        }

        public TaskResource GetTask(DeploymentResource resource)
        {
            return Client.Get<TaskResource>(resource.Link("Task"));
        }

        public ResourceCollection<DeploymentResource> FindBy(string[] projects, string[] environments, int skip = 0, int? take = null)
        {
            return Client.List<DeploymentResource>(Repository.Link("Deployments"), new { skip, take, projects = projects ?? new string[0], environments = environments ?? new string[0] });
        }

        [Obsolete("This method is not a find all, it still requires paging. So it has been renamed to `FindBy`")]
        public ResourceCollection<DeploymentResource> FindAll(string[] projects, string[] environments, int skip = 0, int? take = null)
        {
            return FindBy(projects, environments, skip, take);
        }

        public void Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage)
        {
            Paginate(projects, environments, new string[0], getNextPage);
        }

        public void Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<DeploymentResource>, bool> getNextPage)
        {
            Client.Paginate(Repository.Link("Deployments"), new { projects = projects ?? new string[0], environments = environments ?? new string[0], tenants = tenants ?? new string[0] }, getNextPage);
        }
    }
}