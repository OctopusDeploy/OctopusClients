using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Octo.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;
using Serilog.Core;

namespace Octopus.Cli.Commands
{
    [Command("list-latestdeployments", Description = "List the releases last-deployed in each environment")]
    public class ListLatestDeploymentsCommand : ApiCommand, ISupportFormattedOutput
    {
        readonly HashSet<string> environments = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> projects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        IDictionary<string, string> projectsById;
        string[] projectsFilter;
        IDictionary<string, string> environmentsById;
        string[] environmentsFilter;
        DashboardResource dashboard;
        Dictionary<string, string> tenantsById;
        private Dictionary<DashboardItemResource, DeploymentRelatedResources> dashboardRelatedResourceses;

        public ListLatestDeploymentsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, log, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Listing");
            options.Add("project=", "Name of a project to filter by. Can be specified many times.", v => projects.Add(v));
            options.Add("environment=", "Name of an environment to filter by. Can be specified many times.", v => environments.Add(v));
        }

        private async Task<IDictionary<string, string>> LoadProjects()
        {
            commandOutputProvider.PrintDebugMessage("Loading projects...");
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
            commandOutputProvider.PrintDebugMessage("Loading environments...");
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

        private static void LogDeploymentInfo(ILogger log, DashboardItemResource dashboardItem, ReleaseResource release, ChannelResource channel,
            IDictionary<string, string> environmentsById, IDictionary<string, string> projectedById, IDictionary<string, string> tenantsById)
        {
            var nameOfDeploymentEnvironment = environmentsById[dashboardItem.EnvironmentId];
            var nameOfDeploymentProject = projectedById[dashboardItem.ProjectId];

            log.Information(" - Project: {Project:l}", nameOfDeploymentProject);
            log.Information(" - Environment: {Environment:l}", nameOfDeploymentEnvironment);
            if (!string.IsNullOrEmpty(dashboardItem.TenantId))
            {
                var nameOfDeploymentTenant = tenantsById[dashboardItem.TenantId];
                log.Information(" - Tenant: {Tenant:l}", nameOfDeploymentTenant);
            }

            if(channel != null)
            {
                log.Information(" - Channel: {Channel:l}", channel.Name);
            }

            log.Information("   Date: {$Date:l}", dashboardItem.QueueTime);
            log.Information("   Duration: {Duration:l}", dashboardItem.Duration);

            if (dashboardItem.State == TaskState.Failed)
            {
                log.Error("   State: {$State:l}", dashboardItem.State);
            }
            else
            {
                log.Information("   State: {$State:l}", dashboardItem.State);
            }

            log.Information("   Version: {Version:l}", release.Version);
            log.Information("   Assembled: {$Assembled:l}", release.Assembled);
            log.Information("   Package Versions: {PackageVersion:l}", GetPackageVersionsAsString(release.SelectedPackages));
            log.Information("   Release Notes: {ReleaseNotes:l}", GetReleaseNotes(release));

            log.Information("");
        }

        

        public async Task Query()
        {
            projectsById = await LoadProjects();
            projectsFilter = projectsById.Keys.ToArray();

            environmentsById = await LoadEnvironments();
            environmentsFilter = environmentsById.Keys.ToArray();

            commandOutputProvider.PrintDebugMessage("Loading dashboard...");

            dashboard = await Repository.Dashboards.GetDynamicDashboard(projectsFilter, environmentsFilter).ConfigureAwait(false);
            tenantsById = dashboard.Tenants.ToDictionary(t => t.Id, t => t.Name);

            dashboardRelatedResourceses = new Dictionary<DashboardItemResource, DeploymentRelatedResources>();
            foreach (var dashboardItem in dashboard.Items)
            {
                DeploymentRelatedResources drr = new DeploymentRelatedResources();
                drr.ReleaseResource = await Repository.Releases.Get(dashboardItem.ReleaseId).ConfigureAwait(false);
                
                if (!string.IsNullOrEmpty(dashboardItem.ChannelId))
                    drr.ChannelResource = await Repository.Channels.Get(dashboardItem.ChannelId).ConfigureAwait(false);

                dashboardRelatedResourceses[dashboardItem] = drr;
            }
        }

        public void PrintDefaultOutput()
        {
            if (!dashboard.Items.Any())
            {
                Log.Information("Did not find any releases matching the search criteria.");
            }

            foreach (var dashboardItem in dashboardRelatedResourceses.Keys)
            {
                LogDeploymentInfo(
                    Log, 
                    dashboardItem, 
                    dashboardRelatedResourceses[dashboardItem].ReleaseResource,
                    dashboardRelatedResourceses[dashboardItem].ChannelResource, 
                    environmentsById, 
                    projectsById,
                    tenantsById);
            }
            
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.PrintJsonOutput(dashboardRelatedResourceses.Keys.Select(dashboardItem => new
                {
                    dashboardItem,
                    release = dashboardRelatedResourceses[dashboardItem].ReleaseResource,
                    channel = dashboardRelatedResourceses[dashboardItem].ChannelResource,
                })
                .Select(x => new
                {
                    Project = projectsById[x.dashboardItem.ProjectId],
                    Environment = environmentsById[x.dashboardItem.EnvironmentId],
                    Tenant = !string.IsNullOrEmpty(x.dashboardItem.TenantId)
                        ? tenantsById[x.dashboardItem.TenantId]
                        : string.Empty,
                    Channel = x.channel != null ? x.channel.Name : string.Empty,
                    Date = x.dashboardItem.QueueTime,
                    x.dashboardItem.Duration,
                    State = x.dashboardItem.State.ToString(),
                    x.release.Version,
                    x.release.Assembled,
                    PackageVersion = GetPackageVersionsAsString(x.release.SelectedPackages),
                    ReleaseNotes = GetReleaseNotes(x.release)
                }));
        }

        public void PrintXmlOutput()
        {
            throw new NotImplementedException();
        }
    }
}