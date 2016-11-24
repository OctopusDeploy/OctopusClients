using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands
{
    [Command("list-latestdeployments", Description = "List the releases last-deployed in each environment")]
    public class ListLatestDeploymentsCommand : ApiCommand
    {
        readonly HashSet<string> environments = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> projects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public ListLatestDeploymentsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Listing");
            options.Add("project=", "Name of a project to filter by. Can be specified many times.", v => projects.Add(v));
            options.Add("environment=", "Name of an environment to filter by. Can be specified many times.", v => environments.Add(v));
        }

        protected override async Task Execute()
        {
            var projectsById = await LoadProjects();
            var projectsFilter = projectsById.Keys.ToArray();

            var environmentsById = await LoadEnvironments();
            var environmentsFilter = environmentsById.Keys.ToArray();

            Log.Debug("Loading dashboard...");
            var dashboard = await Repository.Dashboards.GetDynamicDashboard(projectsFilter, environmentsFilter).ConfigureAwait(false);

            Log.Debug("Loading deployments...");
            var deployments = await Repository.Deployments.Get(dashboard.Items.Select(d => d.DeploymentId).ToArray()).ConfigureAwait(false);

            var deploymentsById = deployments.ToDictionary(p => p.Id, p => p);

            var tenantsById = dashboard.Tenants.ToDictionary(t => t.Id, t => t.Name);

            foreach (var item in dashboard.Items)
            {
                await LogDeploymentInfo(deploymentsById[item.DeploymentId], environmentsById, projectsById, tenantsById).ConfigureAwait(false);
            }
        }

        async Task<IDictionary<string, string>> LoadProjects()
        {
            Log.Debug("Loading projects...");

            if (projects.Any())
            {
                var projectResources = Repository.Projects.FindByNames(projects.ToArray()).ConfigureAwait(false);
                return (await projectResources).ToDictionary(p => p.Id, p => p.Name);
            }

            var allProjects = Repository.Projects.FindAll().ConfigureAwait(false);
            return (await allProjects).ToDictionary(p => p.Id, p => p.Name);
        }

        async Task<IDictionary<string, string>> LoadEnvironments()
        {
            Log.Debug("Loading environments...");
            var environmentResources = environments.Count > 0
                ? Repository.Environments.FindByNames(environments.ToArray())
                : Repository.Environments.FindAll();

            return (await environmentResources.ConfigureAwait(false)).ToDictionary(p => p.Id, p => p.Name);
        }

        public async Task LogDeploymentInfo(DeploymentResource deployment, IDictionary<string, string> environmentsById, IDictionary<string, string> projectedById, IDictionary<string, string> tenantsById)
        {
            var nameOfDeploymentEnvironment = environmentsById[deployment.EnvironmentId];
            var nameOfDeploymentProject = projectedById[deployment.ProjectId];
            var task = Repository.Tasks.Get(deployment.Link("Task")).ConfigureAwait(false);
            var release = Repository.Releases.Get(deployment.Link("Release")).ConfigureAwait(false);

            var propertiesToLog = new List<string>();
            propertiesToLog.AddRange(FormatTaskPropertiesAsStrings(await task));
            propertiesToLog.AddRange(FormatReleasePropertiesAsStrings(await release));
            Log.Information(" - Project: {Project:l}", nameOfDeploymentProject);
            Log.Information(" - Environment: {Environment:l}", nameOfDeploymentEnvironment);
            if (!string.IsNullOrEmpty(deployment.TenantId))
            {
                var nameOfDeploymentTenant = tenantsById[deployment.TenantId];
                Log.Information(" - Tenant: {Tenant:l}", nameOfDeploymentTenant);
            }
            foreach (var property in propertiesToLog)
            {
                if (property == "State: Failed")
                    Log.Error("   {Property:l}", property);
                else
                    Log.Information("   {Property:l}", property);
            }
            Log.Information("");
        }

        static IEnumerable<string> FormatTaskPropertiesAsStrings(TaskResource task)
        {
            return new List<string>
            {
                "Date: " + task.QueueTime,
                "Duration: " + task.Duration,
                "State: " + task.State
            };
        }
    }
}