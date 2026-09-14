using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.Triggers;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class ProjectTriggersEditor
    {
        private readonly IProjectTriggerRepository repository;
        private readonly ProjectResource owner;
        private readonly List<ProjectTriggerEditor> trackedProjectTriggerBuilders = new List<ProjectTriggerEditor>();

        public ProjectTriggersEditor(IProjectTriggerRepository repository, ProjectResource owner)
        {
            this.repository = repository;
            this.owner = owner;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectTriggerEditor> CreateOrModify(string name, TriggerFilterResource filter, TriggerActionResource action)
            => CreateOrModify(name, filter, action, CancellationToken.None);

        public async Task<ProjectTriggerEditor> CreateOrModify(string name, TriggerFilterResource filter, TriggerActionResource action, CancellationToken cancellationToken)
        {
            var projectTriggerBuilder = await new ProjectTriggerEditor(repository).CreateOrModify(owner, name, filter, action, cancellationToken).ConfigureAwait(false);
            trackedProjectTriggerBuilders.Add(projectTriggerBuilder);
            return projectTriggerBuilder;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectTriggersEditor> Delete(string name)
            => Delete(name, CancellationToken.None);

        public async Task<ProjectTriggersEditor> Delete(string name, CancellationToken cancellationToken)
        {
            var trigger = await repository.FindByName(owner, name, cancellationToken).ConfigureAwait(false);
            if (trigger != null)
                await repository.Delete(trigger, cancellationToken).ConfigureAwait(false);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectTriggersEditor> SaveAll()
            => SaveAll(CancellationToken.None);

        public async Task<ProjectTriggersEditor> SaveAll(CancellationToken cancellationToken)
        {
            await Task.WhenAll(trackedProjectTriggerBuilders.Select(x => x.Save(cancellationToken))).ConfigureAwait(false);
            return this;
        }
    }
}
