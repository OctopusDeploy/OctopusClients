using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.OpsProcesses;

namespace Octopus.Client.Repositories.Async
{
    public interface IOpsRunRepository : IGet<OpsRunResource>, ICreate<OpsRunResource>, IPaginate<OpsRunResource>, IDelete<OpsRunResource>
    {
        Task<TaskResource> GetTask(OpsRunResource resource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="environments"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        Task<ResourceCollection<OpsRunResource>> FindBy(string[] projects, string[] environments, int skip = 0, int? take = null);

        [Obsolete("This method is not a find all, it still requires paging. So it has been renamed to `FindBy`")]
        Task<ResourceCollection<OpsRunResource>> FindAll(string[] projects, string[] environments, int skip = 0, int? take = null);
        Task Paginate(string[] projects, string[] environments, Func<ResourceCollection<OpsRunResource>, bool> getNextPage);
        Task Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<OpsRunResource>, bool> getNextPage);
    }

    class OpsRunRepository : BasicRepository<OpsRunResource>, IOpsRunRepository
    {
        public OpsRunRepository(IOctopusAsyncRepository repository)
            : base(repository, "OpsRuns")
        {
        }

        public Task<TaskResource> GetTask(OpsRunResource resource)
        {
            return Client.Get<TaskResource>(resource.Link("Task"));
        }

        public async Task<ResourceCollection<OpsRunResource>> FindBy(string[] projects, string[] environments, int skip = 0, int? take = null)
        {
            return await Client.List<OpsRunResource>(await Repository.Link("OpsRuns").ConfigureAwait(false), new { skip, take, projects = projects ?? new string[0], environments = environments ?? new string[0] }).ConfigureAwait(false);
        }

        [Obsolete("This method is not a find all, it still requires paging. So it has been renamed to `FindBy`")]
        public Task<ResourceCollection<OpsRunResource>> FindAll(string[] projects, string[] environments, int skip = 0, int? take = null)
        {
            return FindBy(projects, environments, skip, take);
        }

        public Task Paginate(string[] projects, string[] environments, Func<ResourceCollection<OpsRunResource>, bool> getNextPage)
        {
            return Paginate(projects, environments, new string[0], getNextPage);
        }

        public async Task Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<OpsRunResource>, bool> getNextPage)
        {
            await Client.Paginate(await Repository.Link("OpsRuns").ConfigureAwait(false), new { projects = projects ?? new string[0], environments = environments ?? new string[0], tenants = tenants ?? new string[0] }, getNextPage).ConfigureAwait(false);
        }
    }
}
