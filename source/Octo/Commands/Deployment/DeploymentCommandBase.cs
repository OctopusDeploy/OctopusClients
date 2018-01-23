using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Model.Forms;
using Octostache;
using Serilog;

namespace Octopus.Cli.Commands.Deployment
{
    public abstract class DeploymentCommandBase : ApiCommand
    {
        readonly VariableDictionary variables = new VariableDictionary();
        protected IReadOnlyList<DeploymentResource> deployments;
        protected List<DeploymentPromotionTarget> promotionTargets;
        protected List<TenantResource> deploymentTenants;

        protected DeploymentCommandBase(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            SpecificMachineNames = new List<string>();
            SkipStepNames = new List<string>();
            DeployToEnvironmentNames = new List<string>();
            TenantTags = new List<string>();
            Tenants = new List<string>();
            promotionTargets = new List<DeploymentPromotionTarget>();

            var options = Options.For("Deployment");
            options.Add("progress", "[Optional] Show progress of the deployment", v => { showProgress = true; WaitForDeployment = true; noRawLog = true; });
            options.Add("forcepackagedownload", "[Optional] Whether to force downloading of already installed packages (flag, default false).", v => ForcePackageDownload = true);
            options.Add("waitfordeployment", "[Optional] Whether to wait synchronously for deployment to finish.", v => WaitForDeployment = true);
            options.Add("deploymenttimeout=", "[Optional] Specifies maximum time (timespan format) that the console session will wait for the deployment to finish(default 00:10:00). This will not stop the deployment. Requires --waitfordeployment parameter set.", v => DeploymentTimeout = TimeSpan.Parse(v));
            options.Add("cancelontimeout", "[Optional] Whether to cancel the deployment if the deployment timeout is reached (flag, default false).", v => CancelOnTimeout = true);
            options.Add("deploymentchecksleepcycle=", "[Optional] Specifies how much time (timespan format) should elapse between deployment status checks (default 00:00:10)", v => DeploymentStatusCheckSleepCycle = TimeSpan.Parse(v));
            options.Add("guidedfailure=", "[Optional] Whether to use Guided Failure mode. (True or False. If not specified, will use default setting from environment)", v => UseGuidedFailure = bool.Parse(v));
            options.Add("specificmachines=", "[Optional] A comma-separated list of machines names to target in the deployed environment. If not specified all machines in the environment will be considered.", v => SpecificMachineNames.AddRange(v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim())));
            options.Add("force", "[Optional] If a project is configured to skip packages with already-installed versions, override this setting to force re-deployment (flag, default false).", v => ForcePackageRedeployment = true);
            options.Add("skip=", "[Optional] Skip a step by name", v => SkipStepNames.Add(v));
            options.Add("norawlog", "[Optional] Don't print the raw log of failed tasks", v => noRawLog = true);
            options.Add("rawlogfile=", "[Optional] Redirect the raw log of failed tasks to a file", v => rawLogFile = v);
            options.Add("v|variable=", "[Optional] Values for any prompted variables in the format Label:Value. For JSON values, embedded quotation marks should be escaped with a backslash.", ParseVariable);
            options.Add("deployat=", "[Optional] Time at which deployment should start (scheduled deployment), specified as any valid DateTimeOffset format, and assuming the time zone is the current local time zone.", v => ParseDeployAt(v));
            options.Add("tenant=", "Create a deployment for this tenant; specify this argument multiple times to add multiple tenants or use `*` wildcard to deploy to all tenants who are ready for this release (according to lifecycle).", t => Tenants.Add(t));
            options.Add("tenanttag=", "Create a deployment for tenants matching this tag; specify this argument multiple times to build a query/filter with multiple tags, just like you can in the user interface.", tt => TenantTags.Add(tt));
        }

        protected bool ForcePackageRedeployment { get; set; }
        protected bool ForcePackageDownload { get; set; }
        protected bool? UseGuidedFailure { get; set; }
        protected bool WaitForDeployment { get; set; }
        protected TimeSpan DeploymentTimeout { get; set; } = TimeSpan.FromMinutes(10);
        protected bool CancelOnTimeout { get; set; }
        protected TimeSpan DeploymentStatusCheckSleepCycle { get; set; } = TimeSpan.FromSeconds(10);
        protected List<string> SpecificMachineNames { get; set; }
        protected List<string> SkipStepNames { get; set; }
        protected DateTimeOffset? DeployAt { get; set; }
        public string ProjectName { get; set; }
        public List<string> DeployToEnvironmentNames { get; set; }
        public List<string> Tenants { get; set; }
        public List<string> TenantTags { get; set; }

        private bool IsTenantedDeployment => (Tenants.Any() || TenantTags.Any());

        bool noRawLog;
        bool showProgress;
        string rawLogFile;
        TaskOutputProgressPrinter printer = new TaskOutputProgressPrinter();

        protected override void ValidateParameters()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
            if (IsTenantedDeployment && DeployToEnvironmentNames.Count > 1) throw new CommandException("Please specify only one environment at a time when deploying to tenants.");
            if (Tenants.Contains("*") && (Tenants.Count > 1 || TenantTags.Count > 0)) throw new CommandException("When deploying to all tenants using --tenant=* wildcard no other tenant filters can be provided");

            if (IsTenantedDeployment && !Repository.SupportsTenants())
                throw new CommandException("Your Octopus server does not support tenants, which was introduced in Octopus 3.4. Please upgrade your Octopus server, enable the multi-tenancy feature or remove the --tenant and --tenanttag arguments.");

            base.ValidateParameters();
        }

        DateTimeOffset? ParseDeployAt(string v)
        {
            try
            {
                return DeployAt = DateTimeOffset.Parse(v, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal);
            }
            catch (FormatException fex)
            {
                throw new CommandException($"Could not convert '{v}' to a DateTimeOffset: {fex.Message}");
            }
        }

        private async Task<IReadOnlyList<DeploymentResource>> DeployTenantedRelease(ProjectResource project, ReleaseResource release)
        {
            if (DeployToEnvironmentNames.Count != 1)
                return new List<DeploymentResource>();

            var environment = DeployToEnvironmentNames[0];
            var specificMachineIdsTask = GetSpecificMachines();
            var releaseTemplate = await Repository.Releases.GetTemplate(release).ConfigureAwait(false);
            
            deploymentTenants = await GetTenants(project, environment, release, releaseTemplate).ConfigureAwait(false);
            var specificMachineIds = await specificMachineIdsTask.ConfigureAwait(false);

            LogScheduledDeployment();
            
            var createTasks = deploymentTenants.Select(tenant =>
            {
                var promotion =
                    releaseTemplate.TenantPromotions
                        .First(t => t.Id == tenant.Id).PromoteTo
                        .First(tt => tt.Name.Equals(environment, StringComparison.CurrentCultureIgnoreCase));
                promotionTargets.Add(promotion);
                return CreateDeploymentTask(project, release, promotion, specificMachineIds, tenant);
            });

            return await Task.WhenAll(createTasks).ConfigureAwait(false);
        }

        private void LogScheduledDeployment()
        {
            if (DeployAt != null)
            {
                var now = DateTimeOffset.UtcNow;
                commandOutputProvider.Information("Deployment will be scheduled to start in: {Duration:l}", (DeployAt.Value - now).FriendlyDuration());
            }
        }

        private async Task<ReferenceCollection> GetSpecificMachines()
        {
            var specificMachineIds = new ReferenceCollection();
            if (SpecificMachineNames.Any())
            {
                var machines = await Repository.Machines.FindByNames(SpecificMachineNames).ConfigureAwait(false);
                var missing =
                    SpecificMachineNames.Except(machines.Select(m => m.Name), StringComparer.OrdinalIgnoreCase).ToList();
                if (missing.Any())
                {
                    throw new CommandException("The following specific machines could not be found: " + missing.ReadableJoin());
                }

                specificMachineIds.AddRange(machines.Select(m => m.Id));
            }
            return specificMachineIds;
        }

        protected async Task DeployRelease(ProjectResource project, ReleaseResource release)
        {
            var deploymentsTask = IsTenantedDeployment ?
                DeployTenantedRelease(project, release) : 
                DeployToEnvironments(project, release);
            
            deployments = await deploymentsTask.ConfigureAwait(false);
            if (deployments.Any() && WaitForDeployment)
            {
                await WaitForDeploymentToComplete(deployments, project, release).ConfigureAwait(false);
            }
        }

        private async Task<IReadOnlyList<DeploymentResource>> DeployToEnvironments(ProjectResource project, ReleaseResource release)
        {
            if (DeployToEnvironmentNames.Count == 0)
                return new List<DeploymentResource>();

            var releaseTemplate = await Repository.Releases.GetTemplate(release).ConfigureAwait(false);

            var promotingEnvironments =
                (from environment in DeployToEnvironmentNames.Distinct(StringComparer.CurrentCultureIgnoreCase)
                    let promote = releaseTemplate.PromoteTo.FirstOrDefault(p => string.Equals(p.Name, environment, StringComparison.CurrentCultureIgnoreCase))
                    select new {Name = environment, Promotion = promote}).ToList();

            var unknownEnvironments = promotingEnvironments.Where(p => p.Promotion == null).ToList();
            if (unknownEnvironments.Count > 0)
            {
                throw new CommandException(
                    string.Format(
                        "Release '{0}' of project '{1}' cannot be deployed to {2} not in the list of environments that this release can be deployed to. This may be because a) the environment does not exist or is misspelled, b) The lifecycle has not reached this phase, possibly due to previous deployment failure, c) you don't have permission to deploy to this environment, or d) the environment is not in the list of environments defined by the lifecycle.",
                        release.Version,
                        project.Name,
                        unknownEnvironments.Count == 1
                            ? "environment '" + unknownEnvironments[0].Name + "' because the environment is"
                            : "environments " + string.Join(", ", unknownEnvironments.Select(e => "'" + e.Name + "'")) +
                              " because the environments are"
                        ));
            }

            LogScheduledDeployment();
            var specificMachineIds = await GetSpecificMachines().ConfigureAwait(false);

            var createTasks = promotingEnvironments.Select(promotion => CreateDeploymentTask(project, release, promotion.Promotion, specificMachineIds));
            return await Task.WhenAll(createTasks).ConfigureAwait(false);
        }

        private async Task<List<TenantResource>> GetTenants(ProjectResource project, string environmentName, ReleaseResource release,
            DeploymentTemplateResource releaseTemplate)
        {
            if (!Tenants.Any() && !TenantTags.Any())
            {
                return new List<TenantResource>();
            }

            var deployableTenants = new List<TenantResource>();

            if (Tenants.Contains("*"))
            {
                var tenantPromotions = releaseTemplate.TenantPromotions.Where(
                    tp => tp.PromoteTo.Any(
                        promo => promo.Name.Equals(environmentName, StringComparison.CurrentCultureIgnoreCase))).Select(tp => tp.Id).ToArray();

                var tentats = await Repository.Tenants.Get(tenantPromotions).ConfigureAwait(false);
                deployableTenants.AddRange(tentats);

                commandOutputProvider.Information("Found {NumberOfTenants} Tenants who can deploy {Project:l} {Version:l} to {Environment:l}", deployableTenants.Count, project.Name,release.Version, environmentName);
            }
            else
            {
                if (Tenants.Any())
                {
                    var tenantsByName = await Repository.Tenants.FindByNames(Tenants).ConfigureAwait(false);
                    var missing = tenantsByName == null || !tenantsByName.Any()
                        ? Tenants.ToArray()
                        : Tenants.Except(tenantsByName.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToArray();

                    var tenantsById = await Repository.Tenants.Get(missing).ConfigureAwait(false);

                    missing = tenantsById == null || !tenantsById.Any()
                        ? missing
                        : missing.Except(tenantsById.Select(e => e.Id), StringComparer.OrdinalIgnoreCase).ToArray();

                    if (missing.Any())
                        throw new ArgumentException(
                            $"Could not find the {"tenant" + (missing.Length == 1 ? "" : "s")} {string.Join(", ", missing)} on the Octopus server.");

                    deployableTenants.AddRange(tenantsByName);
                    deployableTenants.AddRange(tenantsById);

                    var unDeployableTenants =
                        deployableTenants.Where(dt => !dt.ProjectEnvironments.ContainsKey(project.Id))
                            .Select(dt => $"'{dt.Name}'")
                            .ToList();
                    if (unDeployableTenants.Any())
                        throw new CommandException(
                            string.Format(
                                "Release '{0}' of project '{1}' cannot be deployed for tenant{2} {3}. This may be because either a) {4} not connected to this project, or b) you do not have permission to deploy {5} to this project.",
                                release.Version,
                                project.Name,
                                unDeployableTenants.Count == 1 ? "" : "s",
                                string.Join(" or ", unDeployableTenants),
                                unDeployableTenants.Count == 1 ? "it is" : "they are",
                                unDeployableTenants.Count == 1 ? "it" : "them"));

                    unDeployableTenants = deployableTenants.Where(dt =>
                    {
                        var tenantPromo = releaseTemplate.TenantPromotions.FirstOrDefault(tp => tp.Id == dt.Id);
                        return tenantPromo == null ||
                               !tenantPromo.PromoteTo.Any(
                                   tdt => tdt.Name.Equals(environmentName, StringComparison.CurrentCultureIgnoreCase));
                    }).Select(dt => $"'{dt.Name}'").ToList();
                    if (unDeployableTenants.Any())
                    {
                        throw new CommandException(
                            string.Format(
                                "Release '{0}' of project '{1}' cannot be deployed for tenant{2} {3} to environment '{4}'. This may be because a) the tenant{2} {5} not connected to this environment, a) the environment does not exist or is misspelled, b) The lifecycle has not reached this phase, possibly due to previous deployment failure,  c) you don't have permission to deploy to this environment, d) the environment is not in the list of environments defined by the lifecycle, or e) {6} unable to deploy to this channel.",
                                release.Version,
                                project.Name,
                                unDeployableTenants.Count == 1 ? "" : "s",
                                string.Join(" or ", unDeployableTenants),
                                environmentName,
                                unDeployableTenants.Count == 1 ? "is" : "are",
                                unDeployableTenants.Count == 1 ? "it is" : "they are"));
                    }
                }

                if (TenantTags.Any())
                {

                    var tenantsByTag = await Repository.Tenants.FindAll(null, TenantTags.ToArray()).ConfigureAwait(false);
                    var deployableByTag = tenantsByTag.Where(dt =>
                    {
                        var tenantPromo = releaseTemplate.TenantPromotions.FirstOrDefault(tp => tp.Id == dt.Id);
                        return tenantPromo != null && tenantPromo.PromoteTo.Any(tdt => tdt.Name.Equals(environmentName, StringComparison.CurrentCultureIgnoreCase));
                    }).Where(tenant => !deployableTenants.Any(deployable => deployable.Id == tenant.Id));
                    deployableTenants.AddRange(deployableByTag);
                }
            }

            if (!deployableTenants.Any())
                throw new CommandException(
                    string.Format(
                        "No tenants are available to be deployed for release '{0}' of project '{1}' to environment '{2}'.  This may be because a) No tenants matched the tags provided b) The tenants that do match are not connected to this project or environment, c) The tenants that do match are not yet able to release to this lifecycle phase, or d) you do not have the appropriate deployment permissions.",
                        release.Version, project.Name, environmentName));


            return deployableTenants;
        }

        private async Task<DeploymentResource> CreateDeploymentTask(ProjectResource project, ReleaseResource release, DeploymentPromotionTarget promotionTarget, ReferenceCollection specificMachineIds, TenantResource tenant = null)
        {
            var preview = await Repository.Releases.GetPreview(promotionTarget).ConfigureAwait(false);

            // Validate skipped steps
            var skip = new ReferenceCollection();
            foreach (var step in SkipStepNames)
            {
                var stepToExecute =
                    preview.StepsToExecute.SingleOrDefault(s => string.Equals(s.ActionName, step, StringComparison.CurrentCultureIgnoreCase));
                if (stepToExecute == null)
                {
                    commandOutputProvider.Warning("No step/action named '{Step:l}' could be found when deploying to environment '{Environment:l}', so the step cannot be skipped.", step, promotionTarget.Name);
                }
                else
                {
                    commandOutputProvider.Debug("Skipping step: {Step:l}", stepToExecute.ActionName);
                    skip.Add(stepToExecute.ActionId);
                }
            }

            // Validate form values supplied
            if (preview.Form != null && preview.Form.Elements != null && preview.Form.Values != null)
            {
                foreach (var element in preview.Form.Elements)
                {
                    var variableInput = element.Control as VariableValue;
                    if (variableInput == null)
                    {
                        continue;
                    }

                    var value = variables.Get(variableInput.Label) ?? variables.Get(variableInput.Name);

                    if (string.IsNullOrWhiteSpace(value) && element.IsValueRequired)
                    {
                        throw new ArgumentException("Please provide a variable for the prompted value " + variableInput.Label);
                    }

                    preview.Form.Values[element.Name] = value;
                }
            }

            // Log step with no machines
            foreach (var previewStep in preview.StepsToExecute)
            {
                if (previewStep.HasNoApplicableMachines)
                {
                    commandOutputProvider.Warning("Warning: there are no applicable machines roles used by step {Step:l}", previewStep.ActionName);
                }
            }

            promotionTargets.Add(promotionTarget);
            var deployment = await Repository.Deployments.Create(new DeploymentResource
            {
                TenantId = tenant?.Id,
                EnvironmentId = promotionTarget.Id,
                SkipActions = skip,
                ReleaseId = release.Id,
                ForcePackageDownload = ForcePackageDownload,
                UseGuidedFailure = UseGuidedFailure.GetValueOrDefault(preview.UseGuidedFailureModeByDefault),
                SpecificMachineIds = specificMachineIds,
                ForcePackageRedeployment = ForcePackageRedeployment,
                FormValues = (preview.Form ?? new Form()).Values,
                QueueTime = DeployAt == null ? null : (DateTimeOffset?) DeployAt.Value
            })
            .ConfigureAwait(false);

            commandOutputProvider.Information("Deploying {Project:l} {Release:} to: {PromotionTarget:l} {Tenant:l}(Guided Failure: {GuidedFailure:l})", project.Name, release.Version, promotionTarget.Name,
                tenant == null ? string.Empty : $"for {tenant.Name} ",
                deployment.UseGuidedFailure ? "Enabled" : "Not Enabled");

            return deployment;
        }

        public async Task WaitForDeploymentToComplete(IReadOnlyList<DeploymentResource> deployments, ProjectResource project, ReleaseResource release)
        {
            var getTasks = deployments.Select(dep => Repository.Tasks.Get(dep.TaskId));
            var deploymentTasks = await Task.WhenAll(getTasks).ConfigureAwait(false);
            if (showProgress && deployments.Count > 1)
            {
                commandOutputProvider.Information("Only progress of the first task ({Task:l}) will be shown", deploymentTasks.First().Name);
            }

            try
            {
                commandOutputProvider.Information("Waiting for {NumberOfTasks} deployment(s) to complete....", deploymentTasks.Length);
                await Repository.Tasks.WaitForCompletion(deploymentTasks.ToArray(), DeploymentStatusCheckSleepCycle.Seconds, DeploymentTimeout, PrintTaskOutput).ConfigureAwait(false);
                var failed = false;
                foreach (var deploymentTask in deploymentTasks)
                {
                    var updated = await Repository.Tasks.Get(deploymentTask.Id).ConfigureAwait(false);
                    if (updated.FinishedSuccessfully)
                    {
                        commandOutputProvider.Information("{Task:l}: {State}", updated.Description, updated.State);
                    }
                    else
                    {
                        commandOutputProvider.Error("{Task:l}: {State}, {Error:l}", updated.Description, updated.State, updated.ErrorMessage);
                        failed = true;

                        if (noRawLog)
                        {
                            continue;
                        }

                        try
                        {
                            var raw = await Repository.Tasks.GetRawOutputLog(updated).ConfigureAwait(false);
                            if (!string.IsNullOrEmpty(rawLogFile))
                            {
                                File.WriteAllText(rawLogFile, raw);
                            }
                            else
                            {
                                commandOutputProvider.Error(raw);
                            }
                        }
                        catch (Exception ex)
                        {
                            commandOutputProvider.Error(ex, "Could not retrieve the raw task log for the failed task.");
                        }
                    }
                }
                if (failed)
                {
                    throw new CommandException("One or more deployment tasks failed.");
                }

                commandOutputProvider.Information("Done!");
            }
            catch (TimeoutException e)
            {
                commandOutputProvider.Error(e.Message);

                await CancelDeploymentOnTimeoutIfRequested(deploymentTasks).ConfigureAwait(false);

                var guidedFailureDeployments =
                    from d in deployments
                    where d.UseGuidedFailure
                    select d;
                if (guidedFailureDeployments.Any())
                {
                    commandOutputProvider.Warning("One or more deployments are using Guided Failure. Use the links below to check if intervention is required:");
                    foreach (var guidedFailureDeployment in guidedFailureDeployments)
                    {
                        var environment = await Repository.Environments.Get(guidedFailureDeployment.Link("Environment")).ConfigureAwait(false);
                        commandOutputProvider.Warning("  - {Environment:l}: {Url:l}", environment.Name, GetPortalUrl(string.Format("/app#/projects/{0}/releases/{1}/deployments/{2}", project.Slug, release.Version, guidedFailureDeployment.Id)));
                    }
                }
                throw new CommandException(e.Message);
            }
        }

        private Task CancelDeploymentOnTimeoutIfRequested(IReadOnlyList<TaskResource> deploymentTasks)
        {
            if (!CancelOnTimeout)
                return Task.WhenAll();

            var tasks = deploymentTasks.Select(async task => {
                commandOutputProvider.Warning("Cancelling deployment task '{Task:l}'", task.Description);
                try
                {
                    await Repository.Tasks.Cancel(task).ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    commandOutputProvider.Error("Failed to cancel deployment task '{Task:l}': {ExceptionMessage:l}", task.Description, ex.Message);
                }
            });
            return Task.WhenAll(tasks);
        }

        Task PrintTaskOutput(TaskResource[] taskResources)
        {
            var task = taskResources.First();
            return printer.Render(Repository, commandOutputProvider, task);
        }

        void ParseVariable(string variable)
        {
            var index = new[] { ':', '=' }.Select(s => variable.IndexOf(s)).Where(i => i > 0).OrderBy(i => i).FirstOrDefault();
            if (index <= 0)
                return;

            var key = variable.Substring(0, index);
            var value = (index >= variable.Length - 1) ? string.Empty : variable.Substring(index + 1);

            variables.Set(key, value);
        }
    }
}
