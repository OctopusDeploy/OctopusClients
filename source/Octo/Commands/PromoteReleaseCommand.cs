using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("promote-release", Description = "Promotes a release.")]
    public class PromoteReleaseCommand : DeploymentCommandBase
    {
        public PromoteReleaseCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(repositoryFactory, log, fileSystem, clientFactory)
        {
            var options = Options.For("Release Promotion");
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("from=", "Name of the environment to get the current deployment from, e.g., Staging", v => FromEnvironmentName = v);
            options.Add("to=|deployto=", "Environment to deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
        }

        public string FromEnvironmentName { get; set; }

        protected override void ValidateParameters()
        {
            if (DeployToEnvironmentNames.Count == 0) throw new CommandException("Please specify an environment using the parameter: --deployto=XYZ");
            if (string.IsNullOrWhiteSpace(FromEnvironmentName)) throw new CommandException("Please specify a source environment name using the parameter: --from=XYZ");

            base.ValidateParameters();
        }

        protected override async Task Execute()
        {
            Log.Debug("Finding project: {Project:l}", ProjectName);
            var project = await Repository.Projects.FindByName(ProjectName).ConfigureAwait(false);
            if (project == null)
                throw new CouldNotFindException("a project named", ProjectName);

            Log.Debug("Finding environment: {Environment:l}", FromEnvironmentName);
            var environment = await Repository.Environments.FindByName(FromEnvironmentName).ConfigureAwait(false);
            if (environment == null)
                throw new CouldNotFindException("an environment named", FromEnvironmentName);

            var dashboard = await Repository.Dashboards.GetDynamicDashboard(new[] {project.Id}, new[] {environment.Id}).ConfigureAwait(false);
            var dashboardItem = dashboard.Items.Where(e => e.EnvironmentId == environment.Id && e.ProjectId == project.Id)
                .OrderByDescending(i => SemanticVersion.Parse(i.ReleaseVersion))
                .FirstOrDefault();

            if (dashboardItem == null)
            {
                throw new CouldNotFindException("latest deployment of the project for this environment. Please check that a deployment for this project/environment exists on the dashboard.");
            }

            Log.Debug("Finding release details for release {Version:l}", dashboardItem.ReleaseVersion);
            var release = await Repository.Projects.GetReleaseByVersion(project, dashboardItem.ReleaseVersion).ConfigureAwait(false);

            await DeployRelease(project, release).ConfigureAwait(false);
        }
    }
}