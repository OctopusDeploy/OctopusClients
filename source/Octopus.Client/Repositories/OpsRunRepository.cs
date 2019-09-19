using System;
using Octopus.Client.Model;
using Octopus.Client.Model.OpsProcesses;

namespace Octopus.Client.Repositories
{
    public interface IOpsRunRepository : IGet<OpsRunResource>, ICreate<OpsRunResource>, IPaginate<OpsRunResource>, IDelete<OpsRunResource>
    {
        TaskResource GetTask(OpsRunResource resource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="environments"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <returns></returns>
        ResourceCollection<OpsRunResource> FindBy(string[] projects, string[] environments, int skip = 0, int? take = null);
        void Paginate(string[] projects, string[] environments, Func<ResourceCollection<OpsRunResource>, bool> getNextPage);
        void Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<OpsRunResource>, bool> getNextPage);

    }

    class OpsRunRepository : BasicRepository<OpsRunResource>, IOpsRunRepository
    {
        public OpsRunRepository(IOctopusRepository repository)
            : base(repository, "OpsRuns")
        {
        }

        public TaskResource GetTask(OpsRunResource resource)
        {
            return Client.Get<TaskResource>(resource.Link("Task"));
        }

        public ResourceCollection<OpsRunResource> FindBy(string[] projects, string[] environments, int skip = 0, int? take = null)
        {
            return Client.List<OpsRunResource>(Repository.Link("OpsRuns"), new { skip, take, projects = projects ?? new string[0], environments = environments ?? new string[0] });
        }

        public void Paginate(string[] projects, string[] environments, Func<ResourceCollection<OpsRunResource>, bool> getNextPage)
        {
            Paginate(projects, environments, new string[0], getNextPage);
        }

        public void Paginate(string[] projects, string[] environments, string[] tenants, Func<ResourceCollection<OpsRunResource>, bool> getNextPage)
        {
            Client.Paginate(Repository.Link("OpsRuns"), new { projects = projects ?? new string[0], environments = environments ?? new string[0], tenants = tenants ?? new string[0] }, getNextPage);
        }
    }
}