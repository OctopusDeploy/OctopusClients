using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Packaging;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("list-latestdeployments", Description = "List the releases last-deployed in each environment")]
    public class ListLatestDeploymentsCommand : ApiCommand
    {
        readonly HashSet<string> environments = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> projects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public ListLatestDeploymentsCommand(IOctopusRepositoryFactory repositoryFactory, Serilog.ILogger log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Listing");
            options.Add("project=", "Name of a project to filter by. Can be specified many times.", v => projects.Add(v));
            options.Add("environment=", "Name of an environment to filter by. Can be specified many times.", v => environments.Add(v));
        }

        protected override void Execute()
        {
            var projectsFilter = new string[0];
            if (projects.Count > 0)
            {
                Log.Debug("Loading projects...");
                projectsFilter = Repository.Projects.FindByNames(projects.ToArray()).Select(p => p.Id).ToArray();
            }

            var environmentsById = new Dictionary<string, string>();
            if (environments.Count > 0)
            {
                Log.Debug("Loading environments...");
                CollectionExtensions.AddRange(environmentsById, Repository.Environments.FindByNames(environments.ToArray()).Select(p => new KeyValuePair<string, string>(p.Id, p.Name)));
            }
            else
            {
                CollectionExtensions.AddRange(environmentsById, Repository.Environments.FindAll().Select(p => new KeyValuePair<string, string>(p.Id, p.Name)));
            }

            var deployments = Repository.Deployments.FindAll(projectsFilter, environments.Count > 0 ? environmentsById.Keys.ToArray() : new string[] {});

            foreach (var deployment in deployments.Items)
            {
                LogDeploymentInfo(deployment, environmentsById);
            }
        }

        public void LogDeploymentInfo(DeploymentResource deployment, Dictionary<string, string> environmentsById)
        {
            var nameOfDeploymentEnvironment = environmentsById[deployment.EnvironmentId];
            var taskId = deployment.Link("Task");
            var task = Repository.Tasks.Get(taskId);
            var release = Repository.Releases.Get(deployment.Link("Release"));

            var propertiesToLog = new List<string>();
            propertiesToLog.AddRange(FormatTaskPropertiesAsStrings(task));
            propertiesToLog.AddRange(FormatReleasePropertiesAsStrings(release));
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