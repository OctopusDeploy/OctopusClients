using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;
using Serilog;

namespace Octopus.Cli.Commands
{
    [Command("list-deployments", Description = "List a number of deployments by project, environment or by tenant")]
    public class ListDeploymentsCommand : ApiCommand
    {
        const int DefaultReturnAmount = 30;
        readonly HashSet<string> environments = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> projects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> tenants = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private int? numberOfResults;

        public ListDeploymentsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log,
            IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Listing");
            options.Add("project=", "[Optional] Name of a project to filter by. Can be specified many times.", v => projects.Add(v));
            options.Add("environment=", "[Optional] Name of an environment to filter by. Can be specified many times.", v => environments.Add(v));
            options.Add("tenant=", "[Optional] Name of a tenant to filter by. Can be specified many times.", v => tenants.Add(v));
            options.Add("number=", $"[Optional] number of results to return, default is {DefaultReturnAmount}", v => numberOfResults = int.Parse(v));
        }

        protected override async Task Execute()
        {
            var projectsById = await LoadProjects();
            var projectsFilter = projectsById.Keys.ToArray();

            var environmentsById = await LoadEnvironments();
            var environmentsFilter = environmentsById.Keys.ToArray();

            var features = await Repository.FeaturesConfiguration.GetFeaturesConfiguration();
            var tenantsFilter = new string[0];
            IDictionary<string, string> tenantsById = new Dictionary<string, string>();
            if (features.IsMultiTenancyEnabled)
            {
                tenantsById = await LoadTenants();
                tenantsFilter = tenants.Any() ? tenantsById.Keys.ToArray() : new string[0];
            }

            Log.Debug("Loading deployments...");
            var deploymentResources = new List<DeploymentResource>();
            var maxResults = numberOfResults ?? DefaultReturnAmount;
            await Repository.Deployments
                .Paginate(projectsFilter, environmentsFilter, tenantsFilter, delegate (ResourceCollection<DeploymentResource> page)
                {
                    if(deploymentResources.Count < maxResults)
                        deploymentResources.AddRange(page.Items.Take(maxResults - deploymentResources.Count));

                    return true;
                })
                .ConfigureAwait(false);

            if (!deploymentResources.Any())
            {
                Log.Information("Did not find any deployments matching the search criteria.");
            }

            Log.Debug($"Showing {deploymentResources.Count} results...");
            foreach (var item in deploymentResources)
            {
                var release = await Repository.Releases.Get(item.ReleaseId).ConfigureAwait(false);
                ChannelResource channel = null;

                if (!string.IsNullOrEmpty(item.ChannelId))
                    channel = await Repository.Channels.Get(item.ChannelId).ConfigureAwait(false);

                LogDeploymentInfo(Log, item, release, channel, environmentsById, projectsById, tenantsById);
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

        private async Task<IDictionary<string, string>> LoadTenants()
        {
            Log.Debug("Loading tenants...");
            var tenantsQuery = tenants.Any()
                ? Repository.Tenants.FindByNames(tenants.ToArray())
                : Repository.Tenants.FindAll();


            var tenantsResources = await tenantsQuery.ConfigureAwait(false);

            var missingTenants = tenants.Except(tenantsResources.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToArray();

            if (missingTenants.Any())
            {
                throw new CommandException("Could not find tenants: " + string.Join(",", missingTenants));
            }

            return tenantsResources.ToDictionary(p => p.Id, p => p.Name);
        }

        private static void LogDeploymentInfo(ILogger log, DeploymentResource deploymentItem, ReleaseResource release, ChannelResource channel,
            IDictionary<string, string> environmentsById, IDictionary<string, string> projectsById, IDictionary<string, string> tenantsById)
        {
            var nameOfDeploymentEnvironment = environmentsById[deploymentItem.EnvironmentId];
            var nameOfDeploymentProject = projectsById[deploymentItem.ProjectId];

            log.Information(" - Project: {Project:l}", nameOfDeploymentProject);
            log.Information(" - Environment: {Environment:l}", nameOfDeploymentEnvironment);

            if (!string.IsNullOrEmpty(deploymentItem.TenantId))
            {
                var nameOfDeploymentTenant = tenantsById[deploymentItem.TenantId];
                log.Information(" - Tenant: {Tenant:l}", nameOfDeploymentTenant);
            }

            if (channel != null)
            {
                log.Information(" - Channel: {Channel:l}", channel.Name);
            }

            log.Information("   Created: {$Date:l}", deploymentItem.Created);

            // Date will have to be fetched from Tasks (they need to be loaded) it doesn't come down with the DeploymentResource
            //log.Information("   Date: {$Date:l}", deploymentItem.QueueTime);

            log.Information("   Version: {Version:l}", release.Version);
            log.Information("   Assembled: {$Assembled:l}", release.Assembled);
            log.Information("   Package Versions: {PackageVersion:l}", GetPackageVersionsAsString(release.SelectedPackages));
            log.Information("   Release Notes: {ReleaseNotes:l}", release.ReleaseNotes != null ? release.ReleaseNotes.Replace(Environment.NewLine, @"\n") : "");

            log.Information("");
        }
    }
}