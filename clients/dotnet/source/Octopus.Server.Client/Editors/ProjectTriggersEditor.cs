using System;
using System.Collections.Generic;
using Octopus.Client.Model;
using Octopus.Client.Model.Triggers;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
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

        public ProjectTriggerEditor CreateOrModify(string name, TriggerFilterResource filter, TriggerActionResource action)
        {
            var projectTriggerBuilder = new ProjectTriggerEditor(repository).CreateOrModify(owner, name, filter, action);
            trackedProjectTriggerBuilders.Add(projectTriggerBuilder);
            return projectTriggerBuilder;
        }

        public ProjectTriggersEditor Delete(string name)
        {
            var trigger = repository.FindByName(owner, name);
            if (trigger != null) repository.Delete(trigger);
            return this;
        }

        public ProjectTriggersEditor SaveAll()
        {
            trackedProjectTriggerBuilders.ForEach(x => x.Save());
            return this;
        }
    }
}