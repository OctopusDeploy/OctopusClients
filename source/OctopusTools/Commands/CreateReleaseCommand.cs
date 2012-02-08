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
        public CreateReleaseCommand(IOctopusClientFactory clientFactory, ILog log) : base(clientFactory, log)
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
            
            var server = Client.Handshake().Execute();

            var project = FindProject(server);

            var environments = FindEnvironments(server);

            var steps = FindStepsForProject(project);

            var selected = new List<SelectedPackage>();
            foreach (var step in steps)
            {
                var version = GetLatestVersion(step);
                selected.Add(version);
            }

            var release = CreateNewRelease(project, selected);
            if (environments != null)
            {
                foreach (var environment in environments)
                    DeployRelease(release, environment);
            }
        }

        void DeployRelease(Release release, DeploymentEnvironment environment)
        {
            var deployment = new Deployment();
            deployment.EnvironmentId = environment.Id;
            deployment.ReleaseId = release.Id;
            deployment.ForceRedeployment = Force;

            var result = Client.Create(release.Link("Deployments"), deployment).Execute();

            Log.InfoFormat("Successfully scheduled release '{0}' for deployment to environment '{1}'" + result.Name, release.Version, environment.Name);
        }

        Release CreateNewRelease(Project project, List<SelectedPackage> latestVersions)
        {
            var version = VersionNumber;
            if (string.IsNullOrWhiteSpace(version))
            {
                version = latestVersions.Select(p => SemanticVersion.Parse(p.NuGetPackageVersion)).OrderByDescending(v => v).First().ToString();
                Log.Warn("A --version parameter was not specified, so we will infer the version number from the packages. The highest version number is: " + version);
            }

            var release = new Release();
            release.Assembled = DateTime.UtcNow;
            release.AssembledBy = Environment.UserName;
            release.Version = version;
            release.SelectedPackages = latestVersions.ToArray();

            Log.Debug("Creating release: " + version);

            var result = Client.Create(project.Link("Releases"), release).Execute();

            Log.Info("Release created successfully!");

            return result;
        }

        SelectedPackage GetLatestVersion(Step step)
        {
            Log.DebugFormat("Getting latest version of package: {0}", step.NuGetPackageId);

            var versions = Client.List<PackageVersion>(step.Link("AvailablePackageVersions")).Execute();

            var latest = versions.FirstOrDefault();
            if (latest == null)
            {
                throw new CommandException("There are no available packages named '{0}'");
            }

            Log.InfoFormat("Latest available version of package '{0}' is '{1}'", step.NuGetPackageId, latest.Version);

            return new SelectedPackage { StepId= step.Id, NuGetPackageVersion = latest.Version };
        }

        Project FindProject(OctopusInstance server)
        {
            Log.DebugFormat("Searching for project '{0}'", ProjectName);

            var projects = Client.List<Project>(server.Link("Projects")).Execute();

            var project = projects.FirstOrDefault(x => string.Equals(x.Name, ProjectName, StringComparison.InvariantCultureIgnoreCase));
            if (project == null)
            {
                throw new ArgumentException(string.Format("A project named '{0}' could not be found.", ProjectName));
            }

            Log.InfoFormat("Found project: {0} [{1}]", project.Name, project.Id);

            return project;
        }

        IEnumerable<DeploymentEnvironment> FindEnvironments(OctopusInstance server)
        {
            if (DeployToEnvironmentNames == null || !DeployToEnvironmentNames.Any())
                return Enumerable.Empty<DeploymentEnvironment>();

            var list = new List<DeploymentEnvironment>();
            var environments = Client.List<DeploymentEnvironment>(server.Link("Environments")).Execute();

            foreach (var environmentName in DeployToEnvironmentNames)
            {
                if (string.IsNullOrWhiteSpace(environmentName))
                    continue;

                Log.DebugFormat("Searching for environment '{0}'", environmentName);

                var environment = environments.FirstOrDefault(x => string.Equals(x.Name, environmentName, StringComparison.InvariantCultureIgnoreCase));
                if (environment == null)
                {
                    throw new ArgumentException(string.Format("An environment named '{0}' could not be found.", environmentName));
                }

                Log.InfoFormat("Found environment: {0} [{1}]", environment.Name, environment.Id);

                list.Add(environment);
            }

            return list;
        }

        IEnumerable<Step> FindStepsForProject(Project project)
        {
            Log.Debug("Getting project steps...");

            var steps = Client.List<Step>(project.Link("Steps")).Execute();

            Log.DebugFormat("Found {0} steps", steps.Count);

            return steps;
        }
    }
}
