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

            var tenantsById = dashboard.Tenants.ToDictionary(t => t.Id, t => t.Name);

            if (!dashboard.Items.Any())
            {
                Log.Information("Did not find any releases matching the search criteria.");
            }

            foreach (var item in dashboard.Items)
            {
                await LogDeploymentInfo(item, environmentsById, projectsById, tenantsById).ConfigureAwait(false);
            }
        }

        async Task<IDictionary<string, string>> LoadProjects()
        {
            Log.Debug("Loading projects...");
            var projectQuery = projects.Any()
                ? Repository.Projects.FindByNames(projects.ToArray())
                : Repository.Projects.FindAll();

            var projectResources = await projectQuery.ConfigureAwait(false);

            var missingProjects = projects.Except(projectResources.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToArray();

            if (missingProjects.Any())
            {
                throw new CommandException("Could not find projects: " + string.Join(",", missingProjects));
            }

            return projectResources.ToDictionary(p => p.Id, p => p.Name);
        }

        async Task<IDictionary<string, string>> LoadEnvironments()
        {
            Log.Debug("Loading environments...");
            var environmentQuery = environments.Any()
                ? Repository.Environments.FindByNames(environments.ToArray())
                : Repository.Environments.FindAll();

            var environmentResources = await environmentQuery.ConfigureAwait(false);

            var missingEnvironments = environments.Except(environmentResources.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToArray();

            if (missingEnvironments.Any())
            {
                throw new CommandException("Could not find environments: " + string.Join(",", missingEnvironments));
            }

            return environmentResources.ToDictionary(p => p.Id, p => p.Name);
        }

        public async Task LogDeploymentInfo(DashboardItemResource dashboardItem, IDictionary<string, string> environmentsById, IDictionary<string, string> projectedById, IDictionary<string, string> tenantsById)
        {
            var nameOfDeploymentEnvironment = environmentsById[dashboardItem.EnvironmentId];
            var nameOfDeploymentProject = projectedById[dashboardItem.ProjectId];
            var release = await Repository.Releases.Get(dashboardItem.ReleaseId).ConfigureAwait(false);

            Log.Information(" - Project: {Project:l}", nameOfDeploymentProject);
            Log.Information(" - Environment: {Environment:l}", nameOfDeploymentEnvironment);
            if (!string.IsNullOrEmpty(dashboardItem.TenantId))
            {
                var nameOfDeploymentTenant = tenantsById[dashboardItem.TenantId];
                Log.Information(" - Tenant: {Tenant:l}", nameOfDeploymentTenant);
            }

            if (!string.IsNullOrEmpty(dashboardItem.ChannelId))
            {
                var channel = await Repository.Channels.Get(dashboardItem.ChannelId).ConfigureAwait(false);
                Log.Information(" - Channel: {Channel:l}", channel.Name);
            }

            Log.Information("   Date: {$Date:l}", dashboardItem.QueueTime);
            Log.Information("   Duration: {Duration:l}", dashboardItem.Duration);

            if (dashboardItem.State == TaskState.Failed)
            {
                Log.Error("   State: {$State:l}", dashboardItem.State);
            }
            else
            {
                Log.Information("   State: {$State:l}", dashboardItem.State);
            }

            Log.Information("   Version: {Version:l}", release.Version);
            Log.Information("   Assembled: {$Assembled:l}", release.Assembled);
            Log.Information("   Package Versions: {PackageVersion:l}", GetPackageVersionsAsString(release.SelectedPackages));
            Log.Information("   Release Notes: {ReleaseNotes:l}", release.ReleaseNotes != null ? release.ReleaseNotes.Replace(Environment.NewLine, @"\n") : "");

            Log.Information("");
        }
    }
}