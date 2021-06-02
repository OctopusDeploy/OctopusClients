using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IProjectGroupRepository : IFindByName<ProjectGroupResource>, IGet<ProjectGroupResource>, ICreate<ProjectGroupResource>, IModify<ProjectGroupResource>, IDelete<ProjectGroupResource>, IGetAll<ProjectGroupResource>
    {
        Task<List<ProjectResource>> GetProjects(ProjectGroupResource projectGroup);
        Task<ProjectGroupEditor> CreateOrModify(string name);
        Task<ProjectGroupEditor> CreateOrModify(string name, string description);
    }

    class ProjectGroupRepository : BasicRepository<ProjectGroupResource>, IProjectGroupRepository
    {
        public ProjectGroupRepository(IOctopusAsyncRepository repository)
            : base(repository, "ProjectGroups")
        {
        }

        public async Task<List<ProjectResource>> GetProjects(ProjectGroupResource projectGroup)
        {
            var resources = new List<ProjectResource>();

            await Client.Paginate<ProjectResource>(projectGroup.Link("Projects"), new { }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            }).ConfigureAwait(false);

            return resources;
        }

        public Task<ProjectGroupEditor> CreateOrModify(string name)
        {
            return new ProjectGroupEditor(this).CreateOrModify(name);
        }

        public Task<ProjectGroupEditor> CreateOrModify(string name, string description)
        {
            return new ProjectGroupEditor(this).CreateOrModify(name, description);
        }
    }
}
