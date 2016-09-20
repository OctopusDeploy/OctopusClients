using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Packaging;
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
            var projectsFilter = new string[0];
            if (projects.Count > 0)
            {
                Log.Debug("Loading projects...");
                var projectResources = await Repository.Projects.FindByNames(projects.ToArray()).ConfigureAwait(false);
                projectsFilter = projectResources.Select(p => p.Id).ToArray();
            }

            Log.Debug("Loading environments...");
            var environmentResources = environments.Count > 0
                ? Repository.Environments.FindByNames(environments.ToArray())
                : Repository.Environments.FindAll();

            var environmentsById = (await environmentResources.ConfigureAwait(false)).ToDictionary(p => p.Id, p => p.Name);

            var deployments = await Repository.Deployments.FindAll(projectsFilter, environments.Count > 0 ? environmentsById.Keys.ToArray() : new string[] { }).ConfigureAwait(false);

            foreach (var deployment in deployments.Items)
            {
                await LogDeploymentInfo(deployment, environmentsById).ConfigureAwait(false);
            }
        }

        public async Task LogDeploymentInfo(DeploymentResource deployment, Dictionary<string, string> environmentsById)
        {
            var nameOfDeploymentEnvironment = environmentsById[deployment.EnvironmentId];
            var task = Repository.Tasks.Get(deployment.Link("Task")).ConfigureAwait(false);
            var release = Repository.Releases.Get(deployment.Link("Release")).ConfigureAwait(false);

            var propertiesToLog = new List<string>();
            propertiesToLog.AddRange(FormatTaskPropertiesAsStrings(await task));
            propertiesToLog.AddRange(FormatReleasePropertiesAsStrings(await release));
            Log.Information(" - Environment: {0}", nameOfDeploymentEnvironment);
            foreach (var property in propertiesToLog)
            {
                if (property == "State: Failed")
                    Log.Error("   {0}", property);
                else
                    Log.Information("   {0}", property);
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