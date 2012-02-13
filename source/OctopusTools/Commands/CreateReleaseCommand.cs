using System;
using System.Collections.Generic;
using System.Linq;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Commands
{
    public class CreateReleaseCommand : ApiCommand
    {
        public CreateReleaseCommand(IOctopusSessionFactory sessionFactory, ILog log) : base(sessionFactory, log)
        {
            DeployToEnvironmentNames = new List<string>();
        }

        public string ProjectName { get; set; }
        public IList<string> DeployToEnvironmentNames { get; set; }
        public string VersionNumber { get; set; }
        public bool Force { get; set; }

        public override OptionSet Options
        {
            get
            {
                var options = base.Options;
                options.Add("project=", "Name of the project", v => ProjectName = v);
                options.Add("deployto=", "[Optional] Environment to automatically deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
                options.Add("version=", "Version number to use for the new release.", v => VersionNumber = v);
                options.Add("force", "Whether to force redeployment of already installed packages (flag, default false).", v => Force = true);
                return options;
            }
        }

        public override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");

            Log.Debug("Finding project: " + ProjectName);
            var project = Session.GetProject(ProjectName);

            Log.Debug("Finding environments...");
            var environments = Session.FindEnvironments(DeployToEnvironmentNames);

            Log.Debug("Finding steps for project...");
            var steps = Session.FindStepsForProject(project);

            Log.Debug("Getting latest package versions for each step...");
            var selected = new List<SelectedPackage>();
            foreach (var step in steps)
            {
                var version = Session.GetLatestPackageForStep(step);
                Log.DebugFormat("{0} - latest: {1}", step.Description, version.NuGetPackageVersion);
                selected.Add(version);
            }

            var versionNumber = VersionNumber;
            if (string.IsNullOrWhiteSpace(versionNumber))
            {
                versionNumber = selected.Select(p => SemanticVersion.Parse(p.NuGetPackageVersion)).OrderByDescending(v => v).First().ToString();
                Log.Warn("A --version parameter was not specified, so a version number was automatically selected based on the highest package version: " + versionNumber);
            }

            Log.Debug("Creating release: " + versionNumber);
            var release = Session.CreateRelease(project, selected, versionNumber);
            Log.Info("Release created successfully!");

            if (environments != null)
            {
                foreach (var environment in environments)
                {
                    var deployment = Session.DeployRelease(release, environment, Force);
                    Log.InfoFormat("Successfully scheduled release '{0}' for deployment to environment '{1}'" + deployment.Name, release.Version, environment.Name);
                }
            }
        }
    }
}
