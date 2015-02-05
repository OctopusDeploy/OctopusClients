using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Client.Model;
using Octopus.Platform.Model;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    [Command("deploy-release", Description = "Deploys a release.")]
    public class DeployReleaseCommand : DeploymentCommandBase
    {
        public DeployReleaseCommand(IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(repositoryFactory, log)
        {
            DeployToEnvironmentNames = new List<string>();
            DeploymentStatusCheckSleepCycle = TimeSpan.FromSeconds(10);
            DeploymentTimeout = TimeSpan.FromMinutes(10);

            var options = Options.For("Deployment");
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("deployto=", "Environment to deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
            options.Add("releaseNumber=|version=", "Version number of the release to deploy. Or specify --version=latest for the latest release.", v => VersionNumber = v);
        }

        public string ProjectName { get; set; }
        public List<string> DeployToEnvironmentNames { get; set; }
        public string VersionNumber { get; set; }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
            if (DeployToEnvironmentNames.Count == 0) throw new CommandException("Please specify an environment using the parameter: --deployto=XYZ");
            if (string.IsNullOrWhiteSpace(VersionNumber)) throw new CommandException("Please specify a release version using the parameter: --version=1.0.0.0");

            Log.Debug("Finding project: " + ProjectName);
            var project = Repository.Projects.FindByName(ProjectName);
            if (project == null)
                throw new CommandException("Could not find a project named: " + ProjectName);

            ReleaseResource releaseToPromote;
            if (string.Equals("latest", VersionNumber, StringComparison.CurrentCultureIgnoreCase))
            {
                Log.Debug("Finding latest release for project");
                releaseToPromote = Repository.Projects.GetReleases(project).Items.OrderByDescending(r => SemanticVersion.Parse(r.Version)).FirstOrDefault();

                if (releaseToPromote == null)
                {
                    throw new CommandException("Could not find the latest release for project " + project.Name);
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