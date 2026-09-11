using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;
using Octopus.Client.Model.Triggers;

namespace Octopus.Client.Repositories.Async
{
    public interface IProjectTriggerRepository : ICreate<ProjectTriggerResource>, IModify<ProjectTriggerResource>, IGet<ProjectTriggerResource>, IDelete<ProjectTriggerResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ProjectTriggerResource> FindByName(ProjectResource project, string name);
        Task<ProjectTriggerResource> FindByName(ProjectResource project, string name, CancellationToken cancellationToken);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action);
        Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<ProjectTriggerResource>> FindByRunbook(params string[] runbookIds);
        Task<ResourceCollection<ProjectTriggerResource>> FindByRunbook(CancellationToken cancellationToken, params string[] runbookIds);
    }

    class ProjectTriggerRepository : BasicRepository<ProjectTriggerResource>, IProjectTriggerRepository
    {
        public ProjectTriggerRepository(IOctopusAsyncRepository repository)
            : base(repository, "ProjectTriggers")
        {
            MinimumCompatibleVersion("2019.11.0");
        }

        public Task<ProjectTriggerResource> FindByName(ProjectResource project, string name)
        {
            return FindByName(project, name, CancellationToken.None);
        }

        public Task<ProjectTriggerResource> FindByName(ProjectResource project, string name, CancellationToken cancellationToken)
        {
            return FindByName(name, path: project.Link("Triggers"), pathParameters: null, cancellationToken: cancellationToken);
        }

        public Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action)
        {
            return CreateOrModify(project, name, filter, action, CancellationToken.None);
        }

        public async Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken).ConfigureAwait(false);

            return await new ProjectTriggerEditor(this).CreateOrModify(project, name, filter, action).ConfigureAwait(false);
        }

        public Task<ResourceCollection<ProjectTriggerResource>> FindByRunbook(params string[] runbookIds)
        {
            return FindByRunbook(CancellationToken.None, runbookIds);
        }

        public async Task<ResourceCollection<ProjectTriggerResource>> FindByRunbook(CancellationToken cancellationToken, params string[] runbookIds)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            return await Client.List<ProjectTriggerResource>(await Repository.Link("Triggers"), new { runbooks = runbookIds }, cancellationToken);
        }
    }
}
