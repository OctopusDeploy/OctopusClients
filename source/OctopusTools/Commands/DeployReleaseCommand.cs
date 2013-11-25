using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Model;
using OctopusTools.Infrastructure;
using log4net;

namespace OctopusTools.Commands
{
    [Command("deploy-release", Description="Deploys a release.")]
    public class DeployReleaseCommand : DeploymentCommandBase
    {


        public DeployReleaseCommand(IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(repositoryFactory, log)
        {
            DeployToEnvironmentNames = new List<string>();
            DeploymentStatusCheckSleepCycle = TimeSpan.FromSeconds(10);
            DeploymentTimeout = TimeSpan.FromMinutes(10);
        }

        
        public string ProjectName { get; set; }
        public IList<string> DeployToEnvironmentNames { get; set; }
        public string VersionNumber { get; set; }
        public bool Force { get; set; }

        protected override void SetOptions(OptionSet options)
        {
            SetCommonOptions(options);
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("deployto=", "Environment to deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
            options.Add("releaseNumber=|version=", "Version number of the release to deploy.", v => VersionNumber = v);
            options.Add("force", "Whether to force redeployment of already installed packages (flag, default false).", v => Force = true);
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
            if (DeployToEnvironmentNames.Count == 0) throw new CommandException("Please specify an environment using the parameter: --deployto=XYZ");
            if (string.IsNullOrWhiteSpace(VersionNumber)) throw new CommandException("Please specify a release version using the parameter: --version=1.0.0.0");

            Log.Debug("Finding project: " + ProjectName);
            var project = Repository.Projects.FindByName(ProjectName);

            Log.Debug("Finding environments...");
            var environments = Repository.Environments.FindByNames(DeployToEnvironmentNames);

            Log.Debug("Finding release: " + VersionNumber);
            var release = Repository.Projects.GetReleaseByVersion(project, VersionNumber);

            if (environments == null || environments.Count <= 0) return;

            DeployRelease(project, release, environments);
        }


    }
}
