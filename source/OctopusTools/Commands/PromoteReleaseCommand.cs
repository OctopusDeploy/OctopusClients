using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Client.Model;
using Octopus.Platform.Model;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    [Command("promote-release", Description = "Promotes a release.")]
    public class PromoteReleaseCommand : DeploymentCommandBase
    {
        public PromoteReleaseCommand(IOctopusRepositoryFactory repositoryFactory, ILog log) : base(repositoryFactory, log)
        {
            DeployToEnvironmentNames = new List<string>();
        }

        public List<string> DeployToEnvironmentNames { get; set; }
        public string ProjectName { get; set; }
        public string FromEnvironmentName { get; set; }

        protected override void SetOptions(OptionSet options)
        {
            SetCommonOptions(options);
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("from=", "Name of the environment to get the current deployment from, e.g., Staging", v => FromEnvironmentName = v);
            options.Add("to=|deployto=", "Environment to deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
            if (DeployToEnvironmentNames.Count == 0) throw new CommandException("Please specify an environment using the parameter: --deployto=XYZ");
            if (string.IsNullOrWhiteSpace(FromEnvironmentName)) throw new CommandException("Please specify a source environment name using the parameter: --from=XYZ");

            Log.Debug("Finding project: " + ProjectName);
            var project = Repository.Projects.FindByName(ProjectName);
            if (project == null)
                throw new CommandException("Could not find a project named: " + ProjectName);

            Log.Debug("Finding environment: " + FromEnvironmentName);
            var environment = Repository.Environments.FindByName(FromEnvironmentName);
            if (environment == null)
                throw new CommandException("Could not find an evironment named: " + FromEnvironmentName);

            var dashboard = Repository.Dashboards.GetDynamicDashboard(new[] {project.Id}, new[] {environment.Id});
            var dashboardItem = dashboard.Items.Where(e => e.EnvironmentId == environment.Id && e.ProjectId == project.Id)
                .OrderByDescending(i => SemanticVersion.Parse(i.ReleaseVersion))
                .FirstOrDefault();

            if (dashboardItem == null)
            {
                throw new CommandException("Could not find the latest deployment of the project for this environment. Please check that a deployment for this project/environment exists on the dashboard.");
            }

            Log.Debug("Finding release details for release " + dashboardItem.ReleaseVersion);
            var release = Repository.Projects.GetReleaseByVersion(project, dashboardItem.ReleaseVersion);

            DeployRelease(project, release, DeployToEnvironmentNames);
        }
    }
}