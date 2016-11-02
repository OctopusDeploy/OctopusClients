using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Model;
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

        public async Task<ProjectTriggerEditor> CreateOrModify(string name, ProjectTriggerType type, params ProjectTriggerConditionEvent[] conditions)
        {
            var projectTriggerBuilder = await new ProjectTriggerEditor(repository).CreateOrModify(owner, name, type, conditions).ConfigureAwait(false);
            trackedProjectTriggerBuilders.Add(projectTriggerBuilder);
            return projectTriggerBuilder;
        }

        public async Task<ProjectTriggersEditor> Delete(string name)
        {
            var trigger = await repository.FindByName(owner, name).ConfigureAwait(false);
            if (trigger != null)
                await repository.Delete(trigger).ConfigureAwait(false);
            return this;
        }

        public async Task<ProjectTriggersEditor> SaveAll()
        {
            await Task.WhenAll(trackedProjectTriggerBuilders.Select(x => x.Save())).ConfigureAwait(false);
            return this;
        }
    }
}