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
    [Command("list-deployments", Description = "List a number of releases deployments by project or by environment")]
    public class ListDeploymentsCommand : ApiCommand
    {
        const int NumberDefault = 30;
        readonly HashSet<string> environments = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> projects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private int? numberOfResults;

        public ListDeploymentsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log,
            IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Listing");
            options.Add("project=", "Name of a project to filter by. Can be specified many times.", v => projects.Add(v));
            options.Add("environment=", "Name of an environment to filter by. Can be specified many times.", v => environments.Add(v));
            options.Add("number=", $"[Optional] number of results to return, default is {NumberDefault}", v => numberOfResults = int.Parse(v));
        }

        protected override async Task Execute()
        {
            var projectsById = await LoadProjects();
            var projectsFilter = projectsById.Keys.ToArray();

            var environmentsById = await LoadEnvironments();
            var environmentsFilter = environmentsById.Keys.ToArray();

            Log.Debug("Loading deployments...");
            var deploymentResources = new List<DeploymentResource>();
            var maxResults = (numberOfResults ?? NumberDefault);
            await Repository.Deployments
                .Paginate(projectsFilter, environmentsFilter, delegate(ResourceCollection<DeploymentResource> page)
                {
                    if(deploymentResources.Count < maxResults)
                        deploymentResources.AddRange(page.Items.Take(maxResults - deploymentResources.Count));

                    return true;
                })
                .ConfigureAwait(false);

            var tenantIds = deploymentResources.Select(t => t.TenantId).ToArray();

            if (!deploymentResources.Any())
            {
                Log.Information("Did not find any deplouments matching the search criteria.");
            }

            Log.Debug($"Showing {deploymentResources.Count} results...");
            foreach (var item in deploymentResources)
            {
                var release = await Repository.Releases.Get(item.ReleaseId).ConfigureAwait(false);
                ChannelResource channel = null;

                if (!string.IsNullOrEmpty(item.ChannelId))
                    channel = await Repository.Channels.Get(item.ChannelId).ConfigureAwait(false);

                var tenantResources = new List<TenantResource>();
                if (tenantIds.Any())
                {
                    tenantResources = await Repository.Tenants.Get(tenantIds).ConfigureAwait(false);
                }

                LogDeploymentInfo(Log, item, release, channel, environmentsById, projectsById, tenantResources);
            }

            if(numberOfResults.HasValue && numberOfResults != deploymentResources.Count)
                Log.Debug($"Please note you asked for {numberOfResults} results, but there were only {deploymentResources.Count} that matched your criteria");
        }

        private async Task<IDictionary<string, string>> LoadProjects()
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


        private async Task<IDictionary<string, string>> LoadEnvironments()
        {
            Log.Debug("Loading environments...");
            var environmentQuery = environments.Any()
                ? Repository.Environments.FindByNames(environments.ToArray())
                : Repository.Environments.FindAll();

            var environmentResources = await environmentQuery.ConfigureAwait(false);

            var missingEnvironments =
                environments.Except(environmentResources.Select(e => e.Name), StringComparer.OrdinalIgnoreCase)
                    .ToArray();

            if (missingEnvironments.Any())
            {
                throw new CommandException("Could not find environments: " + string.Join(",", missingEnvironments));
            }

            return environmentResources.ToDictionary(p => p.Id, p => p.Name);
        }

        private static void LogDeploymentInfo(ILogger log, DeploymentResource deploymentItem, ReleaseResource release, ChannelResource channel,
            IDictionary<string, string> environmentsById, IDictionary<string, string> projectedById, IEnumerable<TenantResource> tenantResources)
        {
            var nameOfDeploymentEnvironment = environmentsById[deploymentItem.EnvironmentId];
            var nameOfDeploymentProject = projectedById[deploymentItem.ProjectId];

            log.Information(" - Project: {Project:l}", nameOfDeploymentProject);
            log.Information(" - Environment: {Environment:l}", nameOfDeploymentEnvironment);

            if (!string.IsNullOrEmpty(deploymentItem.TenantId))
            {
                var nameOfDeploymentTenant = tenantResources.FirstOrDefault(t => t.Id == deploymentItem.TenantId)?.Name;
                log.Information(" - Tenant: {Tenant:l}", nameOfDeploymentTenant);
            }

            if (channel != null)
            {
                log.Information(" - Channel: {Channel:l}", channel.Name);
            }

            log.Information("   Date: {$Date:l}", deploymentItem.QueueTime);

            log.Information("   Version: {Version:l}", release.Version);
            log.Information("   Assembled: {$Assembled:l}", release.Assembled);
            log.Information("   Package Versions: {PackageVersion:l}", GetPackageVersionsAsString(release.SelectedPackages));
            log.Information("   Release Notes: {ReleaseNotes:l}", release.ReleaseNotes != null ? release.ReleaseNotes.Replace(Environment.NewLine, @"\n") : "");

            log.Information("");
        }
    }
}