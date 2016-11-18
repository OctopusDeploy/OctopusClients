using System;
using System.Collections.Generic;
using Octopus.Client.Model;
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

        public ProjectTriggerEditor CreateOrModify(string name, ProjectTriggerType type, IProjectTriggerFilterResource filter, IProjectTriggerActionResource action)
        {
            var projectTriggerBuilder = new ProjectTriggerEditor(repository).CreateOrModify(owner, name, type, filter, action);
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