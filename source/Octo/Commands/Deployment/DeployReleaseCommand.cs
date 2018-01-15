using System;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands.Deployment
{
    [Command("deploy-release", Description = "Deploys a release.")]
    public class DeployReleaseCommand : DeploymentCommandBase, ISupportFormattedOutput
    {
        ProjectResource project;
        ChannelResource channel;
        ReleaseResource releaseToPromote;

        public DeployReleaseCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(repositoryFactory, fileSystem, clientFactory, commandOutputProvider)
        {
            var options = Options.For("Deployment");
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("deployto=", "Environment to deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
            options.Add("releaseNumber=|version=", "Version number of the release to deploy. Or specify --version=latest for the latest release.", v => VersionNumber = v);
            options.Add("channel=", "[Optional] Channel to use when getting the release to deploy", v => ChannelName = v);
            options.Add("updateVariables", "Overwrite the variable snapshot for the release by re-importing the variables from the project", v => UpdateVariableSnapshot = true);
        }

        public string VersionNumber { get; set; }
        public string ChannelName { get; set; }
        public bool UpdateVariableSnapshot { get; set; }


        protected override void ValidateParameters()
        {
            if (DeployToEnvironmentNames.Count == 0) throw new CommandException("Please specify an environment using the parameter: --deployto=XYZ");
            if (string.IsNullOrWhiteSpace(VersionNumber)) throw new CommandException("Please specify a release version using the parameter: --version=1.0.0.0 or --version=latest for the latest release");
            if (!string.IsNullOrWhiteSpace(ChannelName) && !Repository.SupportsChannels()) throw new CommandException("Your Octopus server does not support channels, which was introduced in Octopus 3.2. Please upgrade your Octopus server, or remove the --channel argument.");

            base.ValidateParameters();
        }

        public async Task Request()
        {
            project = await RepositoryCommonQueries.GetProjectByName(ProjectName).ConfigureAwait(false);
            channel = await GetChannel(project).ConfigureAwait(false);
            releaseToPromote = await RepositoryCommonQueries.GetReleaseByVersion(VersionNumber, project, channel).ConfigureAwait(false);

            if (UpdateVariableSnapshot)
            {
                commandOutputProvider.Debug("Updating the release variable snapshot with variables from the project");
                await Repository.Releases.SnapshotVariables(releaseToPromote);
            }

            await DeployRelease(project, releaseToPromote).ConfigureAwait(false);
        }

        private async Task<ChannelResource> GetChannel(ProjectResource project)
        {
            var channel = default(ChannelResource);
            if (!string.IsNullOrWhiteSpace(ChannelName))
            {
                commandOutputProvider.Debug("Finding channel: {Channel:l}", ChannelName);
                var channels = await Repository.Projects.GetChannels(project).ConfigureAwait(false);
                channel = await channels
                    .FindOne(Repository, c => string.Equals(c.Name, ChannelName, StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false);

                if (channel == null)
                    throw new CouldNotFindException("a channel named", ChannelName);
            }
            return channel;
        }

        public void PrintDefaultOutput()
        {
            
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(new
            {
                ProjectName = project.Name,
                releaseToPromote.Version,
                Deployments = deployments.Select(d => new
                {
                    DeploymentId = d.Id,
                    d.ReleaseId,
                    Environment = new
                    {
                        d.EnvironmentId,
                        EnvironmentName = promotionTargets.FirstOrDefault(x => x.Id == d.EnvironmentId)?.Name
                    },
                    d.SkipActions,
                    d.SpecificMachineIds,
                    d.ExcludedMachineIds,
                    d.Created,
                    d.Name,
                    d.QueueTime,
                    Tenant = string.IsNullOrEmpty(d.TenantId)
                        ? null
                        : new { d.TenantId, TenantName = deploymentTenants.FirstOrDefault(x => x.Id == d.TenantId)?.Name },
                    d.UseGuidedFailure
                })
            });
        }
    }
}