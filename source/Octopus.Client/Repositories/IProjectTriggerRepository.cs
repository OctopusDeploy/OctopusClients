using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Triggers;

namespace Octopus.Client.Repositories
{
    public interface IProjectTriggerRepository : ICreate<ProjectTriggerResource>, IModify<ProjectTriggerResource>, IGet<ProjectTriggerResource>, IDelete<ProjectTriggerResource>
    {
        ProjectTriggerResource FindByName(ProjectResource project, string name);
        ProjectTriggerEditor CreateOrModify(ProjectResource project, string name, ProjectTriggerType type, TriggerFilterResource filter, TriggerActionResource action);
    }
}
