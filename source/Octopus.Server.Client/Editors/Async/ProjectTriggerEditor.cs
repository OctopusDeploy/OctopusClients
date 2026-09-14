using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.Triggers;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class ProjectTriggerEditor : IResourceEditor<ProjectTriggerResource, ProjectTriggerEditor>
    {
        private readonly IProjectTriggerRepository repository;

        public ProjectTriggerEditor(IProjectTriggerRepository repository)
        {
            this.repository = repository;
        }

        public ProjectTriggerResource Instance { get; private set; }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action)
            => CreateOrModify(project, name, filter, action, CancellationToken.None);

        public async Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(project, name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new ProjectTriggerResource
                {
                    Name = name,
                    ProjectId = project.Id,
                    Filter = filter,
                    Action = action
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Filter = filter;
                existing.Action = action;
                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        public ProjectTriggerEditor Customize(Action<ProjectTriggerResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectTriggerEditor> Save()
            => Save(CancellationToken.None);

        public async Task<ProjectTriggerEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
