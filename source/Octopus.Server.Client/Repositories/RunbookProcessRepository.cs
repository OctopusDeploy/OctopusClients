using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories
{
    public interface IRunbookProcessRepository : IGet<RunbookProcessResource>, IModify<RunbookProcessResource>
    {
        RunbookSnapshotTemplateResource GetTemplate(RunbookProcessResource runbookProcess);
        
        RunbookProcessResource GetInGit(ProjectResource project, string slug, string gitRef);
        
        RunbookProcessResource ModifyInGit(string gitRef, string commitMessage, RunbookProcessResource instance);
    }
    
    class RunbookProcessRepository : BasicRepository<RunbookProcessResource>, IRunbookProcessRepository
    {
        public RunbookProcessRepository(IOctopusRepository repository)
            : base(repository, "RunbookProcesses")
        {
        }

        public RunbookSnapshotTemplateResource GetTemplate(RunbookProcessResource runbookProcess)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbookProcess.Link("RunbookSnapshotTemplate"));
        }
        
        public RunbookProcessResource GetInGit(ProjectResource project, string slug, string gitRef)
            => GetInGit(project.SpaceId, project.Id, slug, gitRef);

        RunbookProcessResource GetInGit(string spaceId, string projectId, string slug, string gitRef)
        {
            return Client.Get<RunbookProcessResource>($"/api/spaces/{spaceId}/projects/{projectId}/{gitRef}/runbookProcesses/{slug}");
        }
        
        public RunbookProcessResource ModifyInGit(string gitRef, string commitMessage, RunbookProcessResource resource)
        {
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyRunbookProcessCommand>(json);
            command.ChangeDescription = commitMessage;
            
            Client.Put<RunbookProcessResource>($"/api/spaces/{resource.SpaceId}/projects/{resource.ProjectId}/{gitRef}/runbookProcesses/{resource.Id}", resource);
            return GetInGit(resource.SpaceId, resource.ProjectId, resource.Id, gitRef);
        }
    }
}