using System;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IProjectTriggerRepository : ICreate<ProjectTriggerResource>, IModify<ProjectTriggerResource>, IGet<ProjectTriggerResource>, IDelete<ProjectTriggerResource>
    {
        Task<ProjectTriggerResource> FindByName(ProjectResource project, string name);
        Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, ProjectTriggerType type);
    }

    class ProjectTriggerRepository : BasicRepository<ProjectTriggerResource>, IProjectTriggerRepository
    {
        public ProjectTriggerRepository(IOctopusAsyncClient client)
            : base(client, "ProjectTriggers")
        {
        }

        public Task<ProjectTriggerResource> FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Triggers"));
        }

        public Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, ProjectTriggerType type)
        {
            return new ProjectTriggerEditor(this).CreateOrModify(project, name, type);
        }
    }
}
