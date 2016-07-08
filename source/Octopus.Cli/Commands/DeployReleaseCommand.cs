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
            TenantTags = new List<string>();
            Tenants = new List<string>();
            DeploymentStatusCheckSleepCycle = TimeSpan.FromSeconds(10);
            DeploymentTimeout = TimeSpan.FromMinutes(10);

            var options = Options.For("Deployment");
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("deployto=", "Environment to deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
            options.Add("releaseNumber=|version=", "Version number of the release to deploy. Or specify --version=latest for the latest release.", v => VersionNumber = v);
            options.Add("channel=", "[Optional] Channel to use when getting the release to deploy", v => ChannelName = v);
            options.Add("tenant=", "A tenant the deployment will be performed for; specify this argument multiple times to add multiple tenants or use `*` wildcard to deploy to tenants able to deploy.", t => Tenants.Add(t));
            options.Add("tenanttag=", "A tenant tag used to match tenants that the deployment will be performed for; specify this argument multiple times to add multiple tenant tags", tt => TenantTags.Add(tt));
        }

        
        public string VersionNumber { get; set; }
        public string ChannelName { get; set; }
        public List<string> Tenants { get; set; }
        public List<string> TenantTags { get; set; }

        private bool IsTenantedDeployment => (Tenants.Any() || TenantTags.Any());

        protected override void ValidateParameters()
        {
            if (DeployToEnvironmentNames.Count == 0) throw new CommandException("Please specify an environment using the parameter: --deployto=XYZ");
            if (string.IsNullOrWhiteSpace(VersionNumber)) throw new CommandException("Please specify a release version using the parameter: --version=1.0.0.0 or --version=latest for the latest release");
            if (!string.IsNullOrWhiteSpace(ChannelName) && !Repository.SupportsChannels()) throw new CommandException("Your Octopus server does not support channels, which was introduced in Octopus 3.2. Please upgrade your Octopus server, or remove the --channel argument.");
            if (IsTenantedDeployment && DeployToEnvironmentNames.Count > 1) throw new CommandException("Please specify only one environment at a time when deploying to tenants.");
            if (Tenants.Contains("*") && (Tenants.Count > 1 || TenantTags.Count > 0)) throw new CommandException("When deploying to all tenants using --tenant=* wildcard no other tenant filters can be provided");

            base.ValidateParameters();
        }

        protected override void Execute()
        {
            var project = GetProject();
            var channel = GetChannel(project);
            var releaseToPromote = GetRelease(project, channel);

            if (IsTenantedDeployment)
                DeployRelease(project, releaseToPromote, DeployToEnvironmentNames[0], Tenants, TenantTags);
            else
                DeployRelease(project, releaseToPromote, DeployToEnvironmentNames);
        }

        private ReleaseResource GetRelease(ProjectResource project, ChannelResource channel)
        {
            string message;
            ReleaseResource releaseToPromote = null;
            if (string.Equals("latest", VersionNumber, StringComparison.CurrentCultureIgnoreCase))
            {
                message = channel == null
                    ? "latest release for project"
                    : $"latest release in channel '{channel.Name}'";

                Log.Debug($"Finding {message}");

                if (channel == null)
                {
                    releaseToPromote = Repository
                        .Projects
                        .GetReleases(project)
                        .Items // We only need the first page
                        .OrderByDescending(r => SemanticVersion.Parse(r.Version))
                        .FirstOrDefault();
                }
                else
                {
                    Repository.Projects.GetReleases(project).Paginate(Repository, page =>
                    {
                        releaseToPromote = page.Items
                            .OrderByDescending(r => SemanticVersion.Parse(r.Version))
                            .FirstOrDefault(r => r.ChannelId == channel.Id);

                        // If we haven't found one yet, keep paginating
                        return releaseToPromote == null;
                    });
                }
            }
            else
            {
                message = $"release {VersionNumber}";
                Log.Debug($"Finding {message}");
                releaseToPromote = Repository.Projects.GetReleaseByVersion(project, VersionNumber);
            }

            if (releaseToPromote == null)
            {
                throw new CouldNotFindException($"the {message}", project.Name);
            }
            return releaseToPromote;
        }

        private ProjectResource GetProject()
        {
            Log.Debug("Finding project: " + ProjectName);
            var project = Repository.Projects.FindByName(ProjectName);
            if (project == null)
                throw new CouldNotFindException("a project named", ProjectName);
            return project;
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