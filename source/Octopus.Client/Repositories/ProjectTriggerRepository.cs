using System;
using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Triggers;

namespace Octopus.Client.Repositories
{
    public interface IProjectTriggerRepository : ICreate<ProjectTriggerResource>, IModify<ProjectTriggerResource>, IGet<ProjectTriggerResource>, IDelete<ProjectTriggerResource>
    {
        ProjectTriggerResource FindByName(ProjectResource project, string name);
        ProjectTriggerEditor CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action);
        ResourceCollection<ProjectTriggerResource> FindByRunbook(params string[] runbookIds);
    }
    
    class ProjectTriggerRepository : BasicRepository<ProjectTriggerResource>, IProjectTriggerRepository
    {
        public ProjectTriggerRepository(IOctopusRepository repository)
            : base(repository, "ProjectTriggers")
        {
            MinimumCompatibleVersion("2019.11.0");
        }

        public ProjectTriggerResource FindByName(ProjectResource project, string name)
        {
            ThrowIfServerVersionIsNotCompatible();
            
            return FindByName(name, path: project.Link("Triggers"));
        }

        public ProjectTriggerEditor CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action)
        {
            ThrowIfServerVersionIsNotCompatible();
            
            return new ProjectTriggerEditor(this).CreateOrModify(project, name, filter, action);
        }

        public ResourceCollection<ProjectTriggerResource> FindByRunbook(params string[] runbookIds)
        {
            return Client.List<ProjectTriggerResource>(Repository.Link("ProjectTriggers"), new {runbooks = runbookIds});
        }
    }
}