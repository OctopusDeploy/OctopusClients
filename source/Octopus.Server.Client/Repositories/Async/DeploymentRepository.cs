using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentRepository : IGet<DeploymentResource>, ICreate<DeploymentResource>, IPaginate<DeploymentResource>, IDelete<DeploymentResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskResource> GetTask(DeploymentResource resource);
        Task<TaskResource> GetTask(DeploymentResource resource, CancellationToken cancellationToken);

        /// <summary>
        ///
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="environments"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<DeploymentResource>> FindBy(string[] projects, string[] environments, int skip = 0, int? take = null);
        Task<ResourceCollection<DeploymentResource>> FindBy(string[] projects, string[] environments, CancellationToken cancellationToken, int skip = 0, int? take = null);

        [Obsolete("This method is not a find all, it still requires paging. So it has been renamed to `FindBy`")]
        Task<ResourceCollection<DeploymentResource>> FindAll(string[] projects, string[] environments, int skip = 0, int? take = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage);
        Task Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<DeploymentResource>, bool> getNextPage);
        Task Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<DeploymentResource>, bool> getNextPage, CancellationToken cancellationToken);
    }

    class DeploymentRepository : BasicRepository<DeploymentResource>, IDeploymentRepository
    {
        public DeploymentRepository(IOctopusAsyncRepository repository)
            : base(repository, "Deployments")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TaskResource> GetTask(DeploymentResource resource)
        {
            return GetTask(resource, CancellationToken.None);
        }

        public Task<TaskResource> GetTask(DeploymentResource resource, CancellationToken cancellationToken)
        {
            return Client.Get<TaskResource>(resource.Link("Task"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ResourceCollection<DeploymentResource>> FindBy(string[] projects, string[] environments, int skip = 0, int? take = null)
        {
            return FindBy(projects, environments, CancellationToken.None, skip, take);
        }

        public async Task<ResourceCollection<DeploymentResource>> FindBy(string[] projects, string[] environments, CancellationToken cancellationToken, int skip = 0, int? take = null)
        {
            return await Client.List<DeploymentResource>(await Repository.Link("Deployments").ConfigureAwait(false), new { skip, take, projects = projects ?? new string[0], environments = environments ?? new string[0] }, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("This method is not a find all, it still requires paging. So it has been renamed to `FindBy`")]
        public Task<ResourceCollection<DeploymentResource>> FindAll(string[] projects, string[] environments, int skip = 0, int? take = null)
        {
            return FindBy(projects, environments, skip, take);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage)
        {
            return Paginate(projects, environments, getNextPage, CancellationToken.None);
        }

        public Task Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage, CancellationToken cancellationToken)
        {
            return Paginate(projects, environments, new string[0], getNextPage, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<DeploymentResource>, bool> getNextPage)
        {
            return Paginate(projects, environments, tenants, getNextPage, CancellationToken.None);
        }

        public async Task Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<DeploymentResource>, bool> getNextPage, CancellationToken cancellationToken)
        {
            await Client.Paginate(await Repository.Link("Deployments").ConfigureAwait(false), new { projects = projects ?? new string[0], environments = environments ?? new string[0], tenants = tenants ?? new string[0] }, getNextPage, cancellationToken).ConfigureAwait(false);
        }
    }
}
