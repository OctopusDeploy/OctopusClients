using System;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
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

            DeploymentStatusCheckSleepCycle = TimeSpan.FromSeconds(10);
            DeploymentTimeout = TimeSpan.FromMinutes(10);

            var options = Options.For("Deployment");
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("deployto=", "Environment to deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
            options.Add("releaseNumber=|version=", "Version number of the release to deploy. Or specify --version=latest for the latest release.", v => VersionNumber = v);
            options.Add("channel=", "[Optional] Channel to use when getting the release to deploy", v => ChannelName = v);
        }

        
        public string VersionNumber { get; set; }
        public string ChannelName { get; set; }


        protected override void ValidateParameters()
        {
            if (DeployToEnvironmentNames.Count == 0) throw new CommandException("Please specify an environment using the parameter: --deployto=XYZ");
            if (string.IsNullOrWhiteSpace(VersionNumber)) throw new CommandException("Please specify a release version using the parameter: --version=1.0.0.0 or --version=latest for the latest release");
            if (!string.IsNullOrWhiteSpace(ChannelName) && !Repository.SupportsChannels()) throw new CommandException("Your Octopus server does not support channels, which was introduced in Octopus 3.2. Please upgrade your Octopus server, or remove the --channel argument.");

            base.ValidateParameters();
        }

        protected override void Execute()
        {
            var project = RepositoryCommonQueries.GetProjectByName(ProjectName);
            var channel = GetChannel(project);
            var releaseToPromote = RepositoryCommonQueries.GetReleaseByVersion(VersionNumber, project, channel);

            DeployRelease(project, releaseToPromote);
        }

        private ChannelResource GetChannel(ProjectResource project)
        {
            var channel = default(ChannelResource);
            if (!string.IsNullOrWhiteSpace(ChannelName))
            {
                Log.Debug("Finding channel: " + ChannelName);
                channel = Repository.Projects.GetChannels(project)
                    .FindOne(Repository, c => string.Equals(c.Name, ChannelName, StringComparison.OrdinalIgnoreCase));

                if (channel == null)
                    throw new CouldNotFindException("a channel named", ChannelName);
            }
            return channel;
        }
    }
}