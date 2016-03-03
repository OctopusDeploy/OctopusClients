using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("deploy-release", Description = "Deploys a release.")]
    public class DeployReleaseCommand : DeploymentCommandBase
    {
        public DeployReleaseCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
            DeployToEnvironmentNames = new List<string>();
            DeploymentStatusCheckSleepCycle = TimeSpan.FromSeconds(10);
            DeploymentTimeout = TimeSpan.FromMinutes(10);

            var options = Options.For("Deployment");
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("deployto=", "Environment to deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
            options.Add("releaseNumber=|version=", "Version number of the release to deploy. Or specify --version=latest for the latest release.", v => VersionNumber = v);
            options.Add("channel=", "[Optional] Channel to use when getting the release to deploy", v => ChannelName = v);
        }

        public string ProjectName { get; set; }
        public List<string> DeployToEnvironmentNames { get; set; }
        public string VersionNumber { get; set; }
        public string ChannelName { get; set; }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
            if (DeployToEnvironmentNames.Count == 0) throw new CommandException("Please specify an environment using the parameter: --deployto=XYZ");
            if (string.IsNullOrWhiteSpace(VersionNumber)) throw new CommandException("Please specify a release version using the parameter: --version=1.0.0.0");

            Log.Debug("Finding project: " + ProjectName);
            var project = Repository.Projects.FindByName(ProjectName);
            if (project == null)
                throw new CouldNotFindException("a project named", ProjectName);

            var channel = default(ChannelResource);
            if (!string.IsNullOrWhiteSpace(ChannelName))
            {
                Log.Debug("Finding channel: " + ChannelName);
                var channels = Repository.Projects.GetChannels(project).Items;
                channel = channels.SingleOrDefault(c => string.Equals(c.Name, ChannelName, StringComparison.OrdinalIgnoreCase));
                if (channel == null)
                    throw new CouldNotFindException("a channel named", ChannelName);
            }

            ReleaseResource releaseToPromote;
            if (string.Equals("latest", VersionNumber, StringComparison.CurrentCultureIgnoreCase))
            {
                if (channel == null)
                {

                    Log.Debug("Finding latest release for project");
                    releaseToPromote = Repository
                        .Projects
                        .GetReleases(project)
                        .Items
                        .OrderByDescending(r => SemanticVersion.Parse(r.Version))
                        .FirstOrDefault();
                }
                else
                {
                    Log.Debug("Finding latest release for channel");
                    releaseToPromote = Repository
                        .Projects
                        .GetReleases(project)
                        .Items
                        .Where(r => r.ChannelId == channel.Id)
                        .OrderByDescending(r => SemanticVersion.Parse(r.Version))
                        .FirstOrDefault();
                }

                if (releaseToPromote == null)
                {
                    throw new CouldNotFindException("the latest release for project", project.Name);
                }
            }
            else
            {
                Log.Debug("Finding release: " + VersionNumber);
                releaseToPromote = Repository.Projects.GetReleaseByVersion(project, VersionNumber);
            }

            DeployRelease(project, releaseToPromote, DeployToEnvironmentNames);
        }
    }
}