using System.Collections.Generic;
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
        RunbookRunResource RunPublished(string projectName, string runbookName, string environmentName, string tenantName, Dictionary<string, string> formValues = null);
        RunbookRunResource RunPublished(string projectName, string runbookName, string environmentName, Dictionary<string, string> formValues = null);
    }

    class RunbookRepository : BasicRepository<RunbookResource>, IRunbookRepository
    {
        public RunbookRepository(IOctopusRepository repository)
            : base(repository, "Runbooks")
        {
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

        public RunbookRunResource RunPublished(string projectName, string runbookName, string environmentName, string tenantName, Dictionary<string, string> formValues = null)
        {
            return RunPublishedInner(projectName, runbookName, environmentName, tenantName, formValues);
        }

        public RunbookRunResource RunPublished(string projectName, string runbookName, string environmentName, Dictionary<string, string> formValues = null)
        {
            return RunPublishedInner(projectName, runbookName, environmentName, null, formValues);
        }

        private RunbookRunResource RunPublishedInner(string projectName, string runbookName, string environmentName, string tenantName, Dictionary<string, string> formValues)
        {
            var root = Repository.LoadRootDocument();

            RunPublishedRunbookResource runPublished = new RunPublishedRunbookResource()
            {
                ProjectName = projectName,
                RunbookName = runbookName,
                EnvironmentName = environmentName,
                TenantName = tenantName
            };

            if (formValues != null)
                runPublished.FormValues = formValues;
            
            return Client.Post<object, RunbookRunResource>(root.LinkToRunbooksRunPublished(), runPublished);
        }
    }
    
    internal static class RunbookLinkExtensions
    {
        public static string LinkToRunbooksRunPublished(this RootResource root)
        {
            return root.Link("RunbooksRunPublished");
        }
    }
}