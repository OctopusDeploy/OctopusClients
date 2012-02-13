using System;
using System.Collections.Generic;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using log4net;

namespace OctopusTools.Commands
{
    public class DeployReleaseCommand : ApiCommand
    {
        public DeployReleaseCommand(IOctopusSessionFactory sessionFactory, ILog log) : base(sessionFactory, log)
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
                options.Add("deployto=", "Environment to deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
                options.Add("version=", "Version number of the release to deploy.", v => VersionNumber = v);
                options.Add("force", "Whether to force redeployment of already installed packages (flag, default false).", v => Force = true);
                return options;
            }
        }

        public override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
            if (DeployToEnvironmentNames.Count == 0) throw new CommandException("Please specify an environment using the parameter: --deployto=XYZ");
            if (string.IsNullOrWhiteSpace(VersionNumber)) throw new CommandException("Please specify a release version using the parameter: --version=1.0.0.0");

            Log.Debug("Finding project: " + ProjectName);
            var project = Session.GetProject(ProjectName);

            Log.Debug("Finding environments...");
            var environments = Session.FindEnvironments(DeployToEnvironmentNames);

            Log.Debug("Finding release: " + VersionNumber);
            var release = Session.GetRelease(project, VersionNumber);

            foreach (var environment in environments)
            {
                var deployment = Session.DeployRelease(release, environment, Force);
                Log.InfoFormat("Successfully scheduled release '{0}' for deployment to environment '{1}'" + deployment.Name, release.Version, environment.Name);
            }
        }
    }
}
