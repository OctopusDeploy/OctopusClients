using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Model;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands.Deployment
{
    [Command("list-deployments", Description = "List a number of deployments by project, environment or by tenant")]
    public class ListDeploymentsCommand : ApiCommand, ISupportFormattedOutput
    {
        const int DefaultReturnAmount = 30;
        readonly HashSet<string> environments = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> projects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> tenants = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private int? numberOfResults;
        IDictionary<string, string> projectsById;
        IDictionary<string, string> environmentsById;
        string[] projectsFilter;
        string[] environmentsFilter;
        IDictionary<string, string> tenantsById;
        
        private Dictionary<DeploymentResource, DeploymentRelatedResources> deploymentResources;

        public ListDeploymentsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Listing");
            options.Add("project=", "[Optional] Name of a project to filter by. Can be specified many times.", v => projects.Add(v));
            options.Add("environment=", "[Optional] Name of an environment to filter by. Can be specified many times.", v => environments.Add(v));
            options.Add("tenant=", "[Optional] Name of a tenant to filter by. Can be specified many times.", v => tenants.Add(v));
            options.Add("number=", $"[Optional] number of results to return, default is {DefaultReturnAmount}", v => numberOfResults = int.Parse(v));
        }

        public async Task Request()
        {
            projectsById = await LoadProjects();
            projectsFilter = projectsById.Keys.ToArray();
            environmentsById = await LoadEnvironments();
            environmentsFilter = environmentsById.Keys.ToArray();

            var features = await Repository.FeaturesConfiguration.GetFeaturesConfiguration();
            var tenantsFilter = new string[0];

            tenantsById = new Dictionary<string, string>();
            if (features.IsMultiTenancyEnabled)
            {
                tenantsById = await LoadTenants();
                tenantsFilter = tenants.Any() ? tenantsById.Keys.ToArray() : new string[0];
            }

            commandOutputProvider.Debug("Loading deployments..."); 

            deploymentResources = new Dictionary<DeploymentResource, DeploymentRelatedResources>();
            var maxResults = numberOfResults ?? DefaultReturnAmount;
            await Repository.Deployments
                .Paginate(projectsFilter, environmentsFilter, tenantsFilter,
                    delegate(ResourceCollection<DeploymentResource> page)
                    {
                        if (deploymentResources.Count < maxResults)
                        {
                            foreach (var dr in page.Items.Take(maxResults - deploymentResources.Count))
                            {
                                deploymentResources.Add(dr, new DeploymentRelatedResources());
                            }
                        }

                        return true;
                    })
                .ConfigureAwait(false);

            foreach (var item in deploymentResources.Keys)
            {
                deploymentResources[item].ReleaseResource = await Repository.Releases.Get(item.ReleaseId).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(item.ChannelId))
                    deploymentResources[item].ChannelResource = await Repository.Channels.Get(item.ChannelId).ConfigureAwait(false);
            }
        }

        public void PrintDefaultOutput()
        {
            if (!deploymentResources.Any())
            {
                commandOutputProvider.Information("Did not find any deployments matching the search criteria.");
            }

            commandOutputProvider.Debug($"Showing {deploymentResources.Count} results...");

            foreach (var item in deploymentResources.Keys)
            {
                LogDeploymentInfo(commandOutputProvider, item, deploymentResources[item].ReleaseResource, deploymentResources[item].ChannelResource, environmentsById, projectsById, tenantsById);
            }    
            
            if (numberOfResults.HasValue && numberOfResults != deploymentResources.Count)
                commandOutputProvider.Debug($"Please note you asked for {numberOfResults} results, but there were only {deploymentResources.Count} that matched your criteria");
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(
                deploymentResources.Select(dr => new
                {
                    Project = new { Id = dr.Key.ProjectId, Name = projectsById[dr.Key.ProjectId] },
                    Environment = new { Id = dr.Key.EnvironmentId, Name = environmentsById[dr.Key.EnvironmentId] },
                    Tenant = string.IsNullOrWhiteSpace(dr.Key.TenantId)
                        ? null
                        : new { Id = dr.Key.TenantId, Name = tenantsById[dr.Key.TenantId] },
                    Channel = dr.Value.ChannelResource == null ? null : new { Id = dr.Key.ChannelId, dr.Value.ChannelResource.Name },
                    dr.Key.Created,
                    dr.Value.ReleaseResource.Version,
                    dr.Value.ReleaseResource.Assembled,
                    PackageVersions = GetPackageVersionsAsString(dr.Value.ReleaseResource.SelectedPackages),
                    ReleaseNotes = GetReleaseNotes(dr.Value.ReleaseResource)
                }));
        }

        private async Task<IDictionary<string, string>> LoadProjects()
        {
            commandOutputProvider.Information("Loading projects...");
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
            commandOutputProvider.Information("Loading environments...");
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
            commandOutputProvider.Information("Loading tenants..."); 

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

        private static void LogDeploymentInfo(ICommandOutputProvider outputProvider, DeploymentResource deploymentItem, ReleaseResource release, ChannelResource channel,
            IDictionary<string, string> environmentsById, IDictionary<string, string> projectsById, IDictionary<string, string> tenantsById)
        {
            var nameOfDeploymentEnvironment = environmentsById[deploymentItem.EnvironmentId];
            var nameOfDeploymentProject = projectsById[deploymentItem.ProjectId];

            outputProvider.Information(" - Project: {Project:l}", nameOfDeploymentProject);
            outputProvider.Information(" - Environment: {Environment:l}", nameOfDeploymentEnvironment);

            if (!string.IsNullOrEmpty(deploymentItem.TenantId))
            {
                var nameOfDeploymentTenant = tenantsById[deploymentItem.TenantId];
                outputProvider.Information(" - Tenant: {Tenant:l}", nameOfDeploymentTenant);
            }

            if (channel != null)
            {
                outputProvider.Information(" - Channel: {Channel:l}", channel.Name);
            }

            outputProvider.Information("\tCreated: {$Date:l}", deploymentItem.Created);

            // Date will have to be fetched from Tasks (they need to be loaded) it doesn't come down with the DeploymentResource
            //log.Information("   Date: {$Date:l}", deploymentItem.QueueTime);

            outputProvider.Information("\tVersion: {Version:l}", release.Version);
            outputProvider.Information("\tAssembled: {$Assembled:l}", release.Assembled);
            outputProvider.Information("\tPackage Versions: {PackageVersion:l}", GetPackageVersionsAsString(release.SelectedPackages));
            outputProvider.Information("\tRelease Notes: {ReleaseNotes:l}", GetReleaseNotes(release));
            outputProvider.Information(string.Empty);
        }
    }
}