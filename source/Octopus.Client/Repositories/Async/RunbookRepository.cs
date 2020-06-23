using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IRunbookRepository : IFindByName<RunbookResource>, IGet<RunbookResource>, ICreate<RunbookResource>, IModify<RunbookResource>, IDelete<RunbookResource>
    {
        Task<RunbookResource> FindByName(ProjectResource project, string name);
        Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description);
        Task<RunbookSnapshotTemplateResource> GetRunbookSnapshotTemplate(RunbookResource runbook);
        Task<RunbookRunTemplateResource> GetRunbookRunTemplate(RunbookResource runbook);
        Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget);
        Task<RunbookRunResource> Run(RunbookResource runbook, RunbookRunResource runbookRun);
        Task<RunbookRunResource> Run(RunbookResource runbook, RunbookRunParameters runbookRunParameters);
    }

    class RunbookRepository : BasicRepository<RunbookResource>, IRunbookRepository
    {
        private readonly SemanticVersion versionThatIntroducesRunbookRunParameters;

        public RunbookRepository(IOctopusAsyncRepository repository)
            : base(repository, "Runbooks")
        {
            versionThatIntroducesRunbookRunParameters = SemanticVersion.Parse("2020.3.0");
        }

        public Task<RunbookResource> FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Runbooks"));
        }

        public Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description)
        {
            return new RunbookEditor(this, new RunbookProcessRepository(Repository)).CreateOrModify(project, name, description);
        }

        public Task<RunbookSnapshotTemplateResource> GetRunbookSnapshotTemplate(RunbookResource runbook)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbook.Link("RunbookSnapshotTemplate"));
        }

        public Task<RunbookRunTemplateResource> GetRunbookRunTemplate(RunbookResource runbook)
        {
            return Client.Get<RunbookRunTemplateResource>(runbook.Link("RunbookRunTemplate"));
        }

        public Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return Client.Get<RunbookRunPreviewResource>(promotionTarget.Link("RunbookRunPreview"));
        }

        public async Task<RunbookRunResource> Run(RunbookResource runbook, RunbookRunResource runbookRun)
        {
            var serverSupportsRunbookRunParameters = SemanticVersion.Parse((await Repository.LoadRootDocument()).Version) >= versionThatIntroducesRunbookRunParameters;

            return serverSupportsRunbookRunParameters
                ? await Run(runbook, RunbookRunParameters.MapFrom(runbookRun))
                : await Client.Post<object, RunbookRunResource>(runbook.Link("CreateRunbookRun"), runbookRun);
        }

        public async Task<RunbookRunResource> Run(RunbookResource runbook, RunbookRunParameters runbookRunParameters)
        {
            var serverVersion = (await Repository.LoadRootDocument()).Version;
            var serverSupportsRunbookRunParameters = SemanticVersion.Parse(serverVersion) >= versionThatIntroducesRunbookRunParameters;

            if (serverSupportsRunbookRunParameters == false)
                throw new UnsupportedApiVersionException($"This Octopus Deploy server is an older version ({serverVersion}) that does not yet support RunbookRunParameters. " +
                                                         $"Please update your Octopus Deploy server to {versionThatIntroducesRunbookRunParameters.ToString()} to access this feature.");

            return await Client.Post<object, RunbookRunResource>(runbook.Link("CreateRunbookRun"), runbookRunParameters);
        }
    }
}
