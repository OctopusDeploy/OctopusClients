using System;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Triggers;

namespace Octopus.Client.Repositories
{
    public interface IProjectTriggerRepository : ICreate<ProjectTriggerResource>, IModify<ProjectTriggerResource>, IGet<ProjectTriggerResource>, IDelete<ProjectTriggerResource>
    {
        ProjectTriggerResource FindByName(ProjectResource project, string name);
        ProjectTriggerEditor CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action);
    }
    
    class ProjectTriggerRepository : BasicRepository<ProjectTriggerResource>, IProjectTriggerRepository
    {
        public ProjectTriggerRepository(IOctopusRepository repository)
            : base(repository, "ProjectTriggers")
        {
        }

        void CheckServerVersionSupportsNewTriggerModel()
        {
            var versionWhenScheduledTriggersWereChanged = SemanticVersion.Parse("2019.11.1");
            if (SemanticVersion.Parse(Repository.LoadRootDocument().Version) < versionWhenScheduledTriggersWereChanged)
            {
                throw new NotSupportedException("The version of the Octopus Server you are connecting to is not compatible with this version of Octopus.Client. Pleas upgrade your Octopus Server to a version >= 2019.11.1");
            }
        }

        public ProjectTriggerResource FindByName(ProjectResource project, string name)
        {
            CheckServerVersionSupportsNewTriggerModel();
            return FindByName(name, path: project.Link("Triggers"));
        }

        public ProjectTriggerEditor CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action)
        {
            CheckServerVersionSupportsNewTriggerModel();
            return new ProjectTriggerEditor(this).CreateOrModify(project, name, filter, action);
        }
    }
}