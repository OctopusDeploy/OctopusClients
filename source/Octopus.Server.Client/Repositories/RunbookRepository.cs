using System.Linq;
using Octopus.Client.Editors;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories
{
    public interface IRunbookRepository : IFindByName<RunbookResource>, IGet<RunbookResource>, ICreate<RunbookResource>, IModify<RunbookResource>, IDelete<RunbookResource>
    {
        RunbookResource FindByName(ProjectResource project, string name);
        RunbookResource GetInGit(string id, ProjectResource project, string gitRef);
        RunbookResource CreateInGit(RunbookResource resource, string gitRef, string commitMessage);
        RunbookResource ModifyInGit(RunbookResource resource, string gitRef, string commitMessage);
        void DeleteInGit(RunbookResource resource, string gitRef, string commitMessage);
        RunbookEditor CreateOrModify(ProjectResource project, string name, string description);
        RunbookEditor CreateOrModifyInGit(ProjectResource project, string slug, string name, string description, string gitRef, string commitMessage);
        RunbookSnapshotTemplateResource GetRunbookSnapshotTemplate(RunbookResource runbook);
        RunbookRunTemplateResource GetRunbookRunTemplate(RunbookResource runbook);
        RunbookRunPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget);
        RunbookRunResource Run(RunbookResource runbook, RunbookRunResource runbookRun);
        RunbookRunResource[] Run(RunbookResource runbook, RunbookRunParameters runbookRunParameters);
        RunbookRunResource[] RunInGit(RunbookResource runbook, RunbookRunGitParameters runbookRunParameters, string gitRef);
    }

    class RunbookRepository : BasicRepository<RunbookResource>, IRunbookRepository
    {
        private readonly SemanticVersion versionAfterWhichRunbookRunParametersAreAvailable;
        private readonly SemanticVersion integrationTestVersion;

        public RunbookRepository(IOctopusRepository repository)
            : base(repository, "Runbooks")
        {
            integrationTestVersion = SemanticVersion.Parse("0.0.0-local");
            versionAfterWhichRunbookRunParametersAreAvailable = SemanticVersion.Parse("2020.3.1");
        }
        
        public RunbookResource GetInGit(string id, ProjectResource project, string gitRef)
        {
            return GetInGit(id, project.SpaceId, project.Id, gitRef);
        }
        
        RunbookResource GetInGit(string id, string spaceId, string projectId, string gitRef)
            => Client.Get<RunbookResource>($"/api/{spaceId}/projects/{projectId}/{gitRef}/runbooks/{id}");
        
        public RunbookResource CreateInGit(RunbookResource resource, string gitRef, string commitMessage)
        {
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyRunbookCommand>(json);
            command.ChangeDescription = commitMessage;
            
            Client.Post<RunbookResource>($"/api/{resource.SpaceId}/projects/{resource.ProjectId}/{gitRef}/runbooks/", command);
            return GetInGit(resource.Id, resource.SpaceId, resource.ProjectId, gitRef);
        }
        
        public RunbookResource ModifyInGit(RunbookResource resource, string gitRef, string commitMessage)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyRunbookCommand>(json);
            command.ChangeDescription = commitMessage;
            
            Client.Put<RunbookResource>($"/api/{resource.SpaceId}/projects/{resource.ProjectId}/{gitRef}/runbooks/{resource.Id}", command);
            return GetInGit(resource.Id, resource.SpaceId, resource.ProjectId, gitRef);
        }
        
        public void DeleteInGit(RunbookResource resource, string gitRef, string commitMessage)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<DeleteRunbookCommand>(json);
            command.ChangeDescription = commitMessage;
            
            Client.Delete($"/api/{resource.SpaceId}/projects/{resource.ProjectId}/{gitRef}/runbooks/{resource.Id}", command);
        }

        public RunbookResource FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Runbooks"));
        }

        public RunbookEditor CreateOrModify(ProjectResource project, string name, string description)
        {
            return new RunbookEditor(this, new RunbookProcessRepository(Repository)).CreateOrModify(project, name, description);
        }
        
        public RunbookEditor CreateOrModifyInGit(ProjectResource project, string slug, string name, string description, string gitRef, string commitMessage)
        {
            return new RunbookEditor(this, new RunbookProcessRepository(Repository)).CreateOrModifyInGit(project, slug, name, description, gitRef,  commitMessage);
        }

        public RunbookSnapshotTemplateResource GetRunbookSnapshotTemplate(RunbookResource runbook)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbook.Link("RunbookSnapshotTemplate"));
        }

        public RunbookRunTemplateResource GetRunbookRunTemplate(RunbookResource runbook)
        {
            return Client.Get<RunbookRunTemplateResource>(runbook.Link("RunbookRunTemplate"));
        }

        public RunbookRunPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return Client.Get<RunbookRunPreviewResource>(promotionTarget.Link("RunbookRunPreview"));
        }

        private bool ServerSupportsRunbookRunParameters(string version)
        {
            var serverVersion = SemanticVersion.Parse(version);

            // Note: We want to ensure the server version is >= *any* 2020.3.1, including all pre-releases to consider what may be rolled out to Octopus Cloud.
            var preReleaseAgnosticServerVersion =
                new SemanticVersion(serverVersion.Major, serverVersion.Minor, serverVersion.Patch);

            return preReleaseAgnosticServerVersion >= versionAfterWhichRunbookRunParametersAreAvailable ||
                   serverVersion == integrationTestVersion;
        }

        public RunbookRunResource Run(RunbookResource runbook, RunbookRunResource runbookRun)
        {
            var serverSupportsRunbookRunParameters = ServerSupportsRunbookRunParameters(Repository.LoadRootDocument().Version);

            return serverSupportsRunbookRunParameters
                ? Run(runbook, RunbookRunParameters.MapFrom(runbookRun)).FirstOrDefault()
                : Client.Post<object, RunbookRunResource>(runbook.Link("CreateRunbookRun"), runbookRun);
        }

        public RunbookRunResource[] Run(RunbookResource runbook, RunbookRunParameters runbookRunParameters)
        {
            var serverVersion = Repository.LoadRootDocument().Version;
            var serverSupportsRunbookRunParameters = ServerSupportsRunbookRunParameters(serverVersion);

            if (serverSupportsRunbookRunParameters == false)
                throw new UnsupportedApiVersionException($"This Octopus Deploy server is an older version ({serverVersion}) that does not yet support RunbookRunParameters. " +
                                                         "Please update your Octopus Deploy server to 2020.3.* or newer to access this feature.");

            return Client.Post<object, RunbookRunResource[]>(runbook.Link("CreateRunbookRun"), runbookRunParameters);
        }
        
        public RunbookRunResource[] RunInGit(RunbookResource runbook, RunbookRunGitParameters runbookRunParameters, string gitRef)
        {
            var serverVersion = Repository.LoadRootDocument().Version;
            var serverSupportsRunbookRunParameters = ServerSupportsRunbookRunParameters(serverVersion);

            if (serverSupportsRunbookRunParameters == false)
                throw new UnsupportedApiVersionException($"This Octopus Deploy server is an older version ({serverVersion}) that does not yet support RunbookRunParameters. " +
                                                         "Please update your Octopus Deploy server to 2020.3.* or newer to access this feature.");

            return Client.Post<object, RunbookRunResource[]>($"/api/{runbook.SpaceId}/projects/{runbook.ProjectId}/{gitRef}/runbooks/{runbook.Id}/run/v1", runbookRunParameters);
        }
    }
}