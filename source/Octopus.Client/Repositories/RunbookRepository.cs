using System.Linq;
using Octopus.Client.Editors;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IRunbookRepository : IFindByName<RunbookResource>, IGet<RunbookResource>, ICreate<RunbookResource>, IModify<RunbookResource>, IDelete<RunbookResource>
    {
        RunbookResource FindByName(ProjectResource project, string name);
        RunbookEditor CreateOrModify(ProjectResource project, string name, string description);
        RunbookSnapshotTemplateResource GetRunbookSnapshotTemplate(RunbookResource runbook);
        RunbookRunTemplateResource GetRunbookRunTemplate(RunbookResource runbook);
        RunbookRunPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget);
        RunbookRunResource Run(RunbookResource runbook, RunbookRunResource runbookRun);
        RunbookRunResource[] Run(RunbookResource runbook, RunbookRunParameters runbookRunParameters);
    }

    class RunbookRepository : BasicRepository<RunbookResource>, IRunbookRepository
    {
        private readonly SemanticVersion versionThatIntroducesRunbookRunParameters;

        public RunbookRepository(IOctopusRepository repository)
            : base(repository, "Runbooks")
        {
            versionThatIntroducesRunbookRunParameters = SemanticVersion.Parse("2020.3.0");
        }

        public RunbookResource FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Runbooks"));
        }

        public RunbookEditor CreateOrModify(ProjectResource project, string name, string description)
        {
            return new RunbookEditor(this, new RunbookProcessRepository(Repository)).CreateOrModify(project, name, description);
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

        public RunbookRunResource Run(RunbookResource runbook, RunbookRunResource runbookRun)
        {
            var serverSupportsRunbookRunParameters = SemanticVersion.Parse(Repository.LoadRootDocument().Version) >= versionThatIntroducesRunbookRunParameters;

            return serverSupportsRunbookRunParameters
                ? Run(runbook, RunbookRunParameters.MapFrom(runbookRun)).FirstOrDefault()
                : Client.Post<object, RunbookRunResource>(runbook.Link("CreateRunbookRun"), runbookRun);
        }

        public RunbookRunResource[] Run(RunbookResource runbook, RunbookRunParameters runbookRunParameters)
        {
            var serverVersion = Repository.LoadRootDocument().Version;
            var serverSupportsRunbookRunParameters = SemanticVersion.Parse(serverVersion) >= versionThatIntroducesRunbookRunParameters;

            if (serverSupportsRunbookRunParameters == false)
                throw new UnsupportedApiVersionException($"This Octopus Deploy server is an older version ({serverVersion}) that does not yet support RunbookRunParameters. " +
                                                         $"Please update your Octopus Deploy server to {versionThatIntroducesRunbookRunParameters.ToString()} to access this feature.");

            return Client.Post<object, RunbookRunResource[]>(runbook.Link("CreateRunbookRun"), runbookRunParameters);
        }
    }
}