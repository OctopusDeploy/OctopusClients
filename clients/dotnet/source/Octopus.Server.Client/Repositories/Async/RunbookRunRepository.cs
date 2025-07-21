using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IRunbookRunRepository : IGet<RunbookRunResource>, ICreate<RunbookRunResource>, IPaginate<RunbookRunResource>, IDelete<RunbookRunResource>
    {
        Task<TaskResource> GetTask(RunbookRunResource resource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="runbooks"></param>
        /// <param name="environments"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        Task<ResourceCollection<RunbookRunResource>> FindBy(string[] projects, string[] runbooks, string[] environments, int skip = 0, int? take = null);
        Task Paginate(string[] projects, string[] runbooks, string[] environments, Func<ResourceCollection<RunbookRunResource>, bool> getNextPage);
        Task Paginate(string[] projects, string[] runbooks, string[] environments, string[] tenants, Func<ResourceCollection<RunbookRunResource>, bool> getNextPage);
        
        /// <summary>
        /// Retries a specific Runbook Run for a Config as Code Runbook
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        Task<RunbookRunResource> Retry(RunbookRunResource run, CancellationToken cancellationToken);
    }

    class RunbookRunRepository : BasicRepository<RunbookRunResource>, IRunbookRunRepository
    {
        public RunbookRunRepository(IOctopusAsyncRepository repository)
            : base(repository, "RunbookRuns")
        {
        }

        public Task<TaskResource> GetTask(RunbookRunResource resource)
        {
            return Client.Get<TaskResource>(resource.Link("Task"));
        }

        public async Task<ResourceCollection<RunbookRunResource>> FindBy(string[] projects, string[] runbooks, string[] environments, int skip = 0, int? take = null)
        {
            return await Client.List<RunbookRunResource>(await Repository.Link("RunbookRuns").ConfigureAwait(false), new { skip, take, projects = projects ?? new string[0], runbooks = runbooks ?? new string[0], environments = environments ?? new string[0] }).ConfigureAwait(false);
        }

        public Task Paginate(string[] projects, string[] runbooks, string[] environments, Func<ResourceCollection<RunbookRunResource>, bool> getNextPage)
        {
            return Paginate(projects, runbooks, environments, new string[0], getNextPage);
        }

        public async Task Paginate(string[] projects, string[] runbooks, string[] environments, string[] tenants, Func<ResourceCollection<RunbookRunResource>, bool> getNextPage)
        {
            await Client.Paginate(await Repository.Link("RunbookRuns").ConfigureAwait(false), new { projects = projects ?? new string[0], runbooks = runbooks ?? new string[0], environments = environments ?? new string[0], tenants = tenants ?? new string[0] }, getNextPage).ConfigureAwait(false);
        }

        public async Task<RunbookRunResource> Retry(RunbookRunResource run, CancellationToken cancellationToken)
        {
            var route = "~/api/{spaceId}/projects/{projectId}/runbookRuns/{id}/retry/v1";

            return await Client.Post<object, RunbookRunResource>(
                route,
                null,
                new
                {
                    spaceId = run.SpaceId,
                    projectId = run.ProjectId,
                    id = run.Id
                },
                cancellationToken
            ).ConfigureAwait(false);
        }
    }
}
