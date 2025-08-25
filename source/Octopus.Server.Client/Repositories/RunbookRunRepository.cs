using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IRunbookRunRepository : IGet<RunbookRunResource>, ICreate<RunbookRunResource>, IPaginate<RunbookRunResource>, IDelete<RunbookRunResource>
    {
        TaskResource GetTask(RunbookRunResource resource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="runbooks"></param>
        /// <param name="environments"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <returns></returns>
        ResourceCollection<RunbookRunResource> FindBy(string[] projects, string[] runbooks, string[] environments, int skip = 0, int? take = null);
        void Paginate(string[] projects, string[] runbooks, string[] environments, Func<ResourceCollection<RunbookRunResource>, bool> getNextPage);
        void Paginate(string[] projects, string[] runbooks, string[] environments, string[] tenants, Func<ResourceCollection<RunbookRunResource>, bool> getNextPage);

        /// <summary>
        /// Retries a specific Runbook Run for a Config as Code Runbook
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        RunbookRunResource Retry(RunbookRunResource run);
    }

    class RunbookRunRepository : BasicRepository<RunbookRunResource>, IRunbookRunRepository
    {
        public RunbookRunRepository(IOctopusRepository repository)
            : base(repository, "RunbookRuns")
        {
        }

        public TaskResource GetTask(RunbookRunResource resource)
        {
            return Client.Get<TaskResource>(resource.Link("Task"));
        }

        public ResourceCollection<RunbookRunResource> FindBy(string[] projects, string[] runbooks, string[] environments, int skip = 0, int? take = null)
        {
            return Client.List<RunbookRunResource>(Repository.Link("RunbookRuns"), new { skip, take, projects = projects ?? new string[0], runbooks = runbooks ?? new string[0], environments = environments ?? new string[0] });
        }

        public void Paginate(string[] projects, string[] runbooks, string[] environments, Func<ResourceCollection<RunbookRunResource>, bool> getNextPage)
        {
            Paginate(projects, runbooks, environments, new string[0], getNextPage);
        }

        public void Paginate(string[] projects, string[] runbooks, string[] environments, string[] tenants, Func<ResourceCollection<RunbookRunResource>, bool> getNextPage)
        {
            Client.Paginate(Repository.Link("RunbookRuns"), new { projects = projects ?? new string[0], runbooks = runbooks ?? new string[0], environments = environments ?? new string[0], tenants = tenants ?? new string[0] }, getNextPage);
        }

        public RunbookRunResource Retry(RunbookRunResource run)
        {
            var route = "~/api/{spaceId}/projects/{projectId}/runbookRuns/{id}/retry/v1";

            return Client.Post<object, RunbookRunResource>(
                route,
                null,
                new
                {
                    spaceId = run.SpaceId,
                    projectId = run.ProjectId,
                    id = run.Id
                }
            );
        }
    }
}