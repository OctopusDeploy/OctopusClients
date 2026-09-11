using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IProjectGroupRepository : IFindByName<ProjectGroupResource>, IGet<ProjectGroupResource>, ICreate<ProjectGroupResource>, IModify<ProjectGroupResource>, IDelete<ProjectGroupResource>, IGetAll<ProjectGroupResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<ProjectResource>> GetProjects(ProjectGroupResource projectGroup);
        Task<List<ProjectResource>> GetProjects(ProjectGroupResource projectGroup, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ProjectGroupEditor> CreateOrModify(string name);
        Task<ProjectGroupEditor> CreateOrModify(string name, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ProjectGroupEditor> CreateOrModify(string name, string description);
        Task<ProjectGroupEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken);
    }

    class ProjectGroupRepository : BasicRepository<ProjectGroupResource>, IProjectGroupRepository
    {
        public ProjectGroupRepository(IOctopusAsyncRepository repository)
            : base(repository, "ProjectGroups")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<ProjectResource>> GetProjects(ProjectGroupResource projectGroup)
            => GetProjects(projectGroup, CancellationToken.None);

        public async Task<List<ProjectResource>> GetProjects(ProjectGroupResource projectGroup, CancellationToken cancellationToken)
        {
            var resources = new List<ProjectResource>();

            await Client.Paginate<ProjectResource>(projectGroup.Link("Projects"), new { }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            }, cancellationToken).ConfigureAwait(false);

            return resources;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectGroupEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public Task<ProjectGroupEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            return new ProjectGroupEditor(this).CreateOrModify(name);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectGroupEditor> CreateOrModify(string name, string description)
            => CreateOrModify(name, description, CancellationToken.None);

        public Task<ProjectGroupEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            return new ProjectGroupEditor(this).CreateOrModify(name, description);
        }
    }
}
