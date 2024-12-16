using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories.Async
{
    public interface IRunbookProcessRepository : IGet<RunbookProcessResource>, IModify<RunbookProcessResource>
    {
        Task<RunbookSnapshotTemplateResource> GetTemplate(RunbookProcessResource runbookProcess);

        Task<RunbookProcessResource> GetInGit(ProjectResource project, string slug, string gitRef, CancellationToken cancellationToken);
        
        Task<RunbookProcessResource> ModifyInGit(string gitRef, string commitMessage, RunbookProcessResource instance,
            CancellationToken cancellationToken);

    }

    class RunbookProcessRepository : BasicRepository<RunbookProcessResource>, IRunbookProcessRepository
    {
        public RunbookProcessRepository(IOctopusAsyncRepository repository)
            : base(repository, "RunbookProcesses")
        {
        }

        public Task<RunbookSnapshotTemplateResource> GetTemplate(RunbookProcessResource runbookProcess)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbookProcess.Link("RunbookSnapshotTemplate"));
        }

        public async Task<RunbookProcessResource> GetInGit(ProjectResource project, string slug, string gitRef, CancellationToken cancellationToken)
            => await GetInGit(project.SpaceId, project.Id, slug, gitRef, cancellationToken);

        async Task<RunbookProcessResource> GetInGit(string spaceId, string projectId, string slug, string gitRef, CancellationToken cancellationToken)
        {
            return await Client.Get<RunbookProcessResource>($"/api/spaces/{spaceId}/projects/{projectId}/{gitRef}/runbookProcesses/{slug}", cancellationToken);
        }
        
        public async Task<RunbookProcessResource> ModifyInGit(string gitRef, string commitMessage, RunbookProcessResource resource, CancellationToken cancellationToken)
        {
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyRunbookProcessCommand>(json);
            command.ChangeDescription = commitMessage;
            
            await Client.Put<RunbookProcessResource>($"/api/spaces/{resource.SpaceId}/projects/{resource.ProjectId}/{gitRef}/runbookProcesses/{resource.Id}", resource, cancellationToken);
            return await GetInGit(resource.SpaceId, resource.ProjectId, resource.Id, gitRef, cancellationToken);
        }
    }
}
