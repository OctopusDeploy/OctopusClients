using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;
using System.Text;

namespace OctopusTools.Commands
{
    public class CreateReleaseCommand : ApiCommand
    {
        readonly IDeploymentWatcher deploymentWatcher;

        public CreateReleaseCommand(IOctopusSessionFactory session, ILog log, IDeploymentWatcher deploymentWatcher)
            : base(session, log)
        {
            this.deploymentWatcher = deploymentWatcher;

            DeployToEnvironmentNames = new List<string>();
            DeploymentStatusCheckSleepCycle = TimeSpan.FromSeconds(10);
            DeploymentTimeout = TimeSpan.FromMinutes(10);
            PackageVersionNumberOverrides = new Dictionary<string, string>();
        }

        public string ProjectName { get; set; }
        public IList<string> DeployToEnvironmentNames { get; set; }
        public string VersionNumber { get; set; }
        public string PackageVersionNumber { get; set; }
        public string ReleaseNotes { get; set; }
        public bool Force { get; set; }
        public bool WaitForDeployment { get; set; }
        public TimeSpan DeploymentTimeout { get; set; }
        public TimeSpan DeploymentStatusCheckSleepCycle { get; set; }
        public IDictionary<string, string> PackageVersionNumberOverrides { get; set; }

        public override OptionSet Options
        {
            get
            {
                var options = base.Options;
                options.Add("project=", "Name of the project", v => ProjectName = v);
                options.Add("deployto=", "[Optional] Environment to automatically deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
                options.Add("version=", "Version number to use for the new release.", v => VersionNumber = v);
                options.Add("packageversion=", "Version number of the package to use for this release.", v => PackageVersionNumber = v);
                options.Add("packageversionoverride=", "[Optional] Version number to use for a package in the release.", ParsePackageConstraint);
                options.Add("force", "Whether to force redeployment of already installed packages (flag, default false).", v => Force = true);
                options.Add("releasenotes=", "Release Notes for the new release.", v => ReleaseNotes = v);
                options.Add("releasenotesfile=", "Path to a file that contains Release Notes for the new release.", ReadReleaseNotesFromFile);
                options.Add("waitfordeployment", "Whether to wait synchronously for deployment to finish.", v => WaitForDeployment = true );
                options.Add("deploymenttimeout=", "[Optional] Specifies maximum time (timespan format) that deployment can take (default 00:10:00)", v => DeploymentTimeout = TimeSpan.Parse(v));
                options.Add("deploymentchecksleepcycle=", "[Optional] Specifies how much time (timespan format) should elapse between deployment status checks (default 00:00:10)", v => DeploymentStatusCheckSleepCycle = TimeSpan.Parse(v));
                return options;
            }
        }

        private void ReadReleaseNotesFromFile(string value)
        {
            try
            {
                ReleaseNotes = File.ReadAllText(value);
            }
            catch (IOException ex)
            {
                throw new CommandException(ex.Message);
            }
        }

        public void ParsePackageConstraint(string value)
        {
            try
            {
                var packageIdAndVersion = PackageConstraintExtensions.GetPackageIdAndVersion(value);

                if (PackageVersionNumberOverrides.ContainsKey(packageIdAndVersion[0]))
                {
                    throw new ArgumentException(string.Format("More than one constraint was specified for package {0}", packageIdAndVersion[0]));
                }

                PackageVersionNumberOverrides.Add(packageIdAndVersion[0], packageIdAndVersion[1]);
            }
            catch (ArgumentException ex)
            {
                throw new CommandException(ex.Message);
            }
        }

        public string GetPackageVersionForStep(Step step)
        {
            if (PackageVersionNumberOverrides != null && PackageVersionNumberOverrides.ContainsKey(step.NuGetPackageId))
            {
                return PackageVersionNumberOverrides[step.NuGetPackageId];
            }
            if (!string.IsNullOrEmpty(PackageVersionNumber))
            {
                return PackageVersionNumber;
            }

            return null;
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

            Log.Debug("Getting package versions for each step...");
            var selected = new List<SelectedPackage>();
            foreach (var step in steps)
            {

                SelectedPackage version;
                var packageVersionConstraint = GetPackageVersionForStep(step);
                if (packageVersionConstraint != null)
                {
                    version = Session.GetPackageForStep(step, packageVersionConstraint);
                }
                else
                {
                    version = Session.GetLatestPackageForStep(step);
                }

                Log.DebugFormat("{0} - latest: {1}", step.Description, version.NuGetPackageVersion);
                selected.Add(version);
            }

            var versionNumber = VersionNumber;
            if (string.IsNullOrWhiteSpace(versionNumber))
            {
                var releases = Session.List<Release>(project.Link("Releases"));
                var lastestReleaseVersion = releases.Select(p => SemanticVersion.Parse(p.Version)).OrderByDescending(v => v).FirstOrDefault();

                if (lastestReleaseVersion != null)
                {
                    var incrementedReleaseVersion = new SemanticVersion(new Version(lastestReleaseVersion.Version.Major, lastestReleaseVersion.Version.Minor, lastestReleaseVersion.Version.Build + 1), lastestReleaseVersion.SpecialVersion);
                    versionNumber = incrementedReleaseVersion.ToString();
                    Log.Warn("A --version parameter was not specified, so a version number was automatically selected based on the lastest release version: " + versionNumber);
                }
                else
                {
                    var highestPackageVersion = selected.Select(p => SemanticVersion.Parse(p.NuGetPackageVersion)).OrderByDescending(v => v).First();
                    versionNumber = highestPackageVersion.ToString();
                    Log.Warn("A --version parameter was not specified and there's not current release version available, so a version number was automatically selected based on the highest package version: " + versionNumber);
                }
            }
            else
            {
                SemanticVersion semVersion = null;

                if (!SemanticVersion.TryParse(versionNumber, out semVersion))
                {
                    versionNumber = ParseVersionMask(versionNumber, project);
                }
            }

            Log.Debug("Creating release: " + versionNumber);
            var release = Session.CreateRelease(project, selected, versionNumber, ReleaseNotes);
            Log.Info("Release created successfully!");

            if (environments != null)
            {
                var linksToDeploymentTasks = RequestDeployments(release, environments);

                if (WaitForDeployment)
                {
                    deploymentWatcher.WaitForDeploymentsToFinish(Session, linksToDeploymentTasks, DeploymentTimeout, DeploymentStatusCheckSleepCycle);
                }
            }

        }

        /// <summary>
        /// Parse version based on mask tokens:
        /// i - Increment based on lastest version
        /// c - Use the lastest version current value
        /// </summary>
        /// <param name="versionNumberMask"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        private string ParseVersionMask(string versionNumberMask, Project project)
        {
            var semanticVersionTokens = versionNumberMask.ToLower().Split('-');
            var versionTokens = semanticVersionTokens[0].Split('.');

            string specialVersionToken = null;
            if (semanticVersionTokens.Length > 1)
            {
                specialVersionToken = semanticVersionTokens[1];
            }

            var releases = Session.List<Release>(project.Link("Releases"));
            var lastestReleaseVersion = releases.Select(p => SemanticVersion.Parse(p.Version)).OrderByDescending(v => v).FirstOrDefault();

            var parsedVersionBuilder = new StringBuilder();

            for (int i = 0; i < versionTokens.Length; i++)
            {
                var versionToken = versionTokens[i];

                int versionComponent;

                if (!int.TryParse(versionToken, out versionComponent))
                {
                    int lastestVersionComponent = 0;

                    if (lastestReleaseVersion != null)
                    {
                        switch (i)
                        {
                            case 0:
                                lastestVersionComponent = lastestReleaseVersion.Version.Major;
                                break;
                            case 1:
                                lastestVersionComponent = lastestReleaseVersion.Version.Minor;
                                break;
                            case 2:
                                lastestVersionComponent = lastestReleaseVersion.Version.Build;
                                break;
                            case 3:
                                lastestVersionComponent = lastestReleaseVersion.Version.Revision;
                                break;
                        }
                    }

                    if (versionToken.Equals("c"))
                    {
                        // Current version
                        versionComponent = lastestVersionComponent;

                    }
                    else if (versionToken.Equals("i"))
                    {
                        // Incremented version
                        versionComponent = lastestVersionComponent + 1;
                    }
                    else
                    {
                        // Invalid token, puts 0 on that part
                        versionComponent = 0;
                    }
                }

                parsedVersionBuilder.Append(versionComponent);
                if (i + 1 < versionTokens.Length)
                {
                    parsedVersionBuilder.Append(".");
                }
            }

            // Parses SemVer special version token
            if (!string.IsNullOrWhiteSpace(specialVersionToken))
            {
                string parsedSpecialVersionToken = null;

                if (specialVersionToken.Equals("c") &&
                    lastestReleaseVersion != null)
                {
                    parsedSpecialVersionToken = lastestReleaseVersion.SpecialVersion;
                }
                else if (specialVersionToken.Equals("i") &&
                         lastestReleaseVersion != null &&
                         !string.IsNullOrWhiteSpace(lastestReleaseVersion.SpecialVersion))
                {
                    int subVersionNumber = 0;

                    var lastestSpecialVersionTokens = lastestReleaseVersion.SpecialVersion.Split('.');

                    if (lastestSpecialVersionTokens.Length > 1)
                    {
                        int.TryParse(lastestSpecialVersionTokens[1], out subVersionNumber);
                    }

                    parsedSpecialVersionToken = string.Format("{0}.{1}", lastestSpecialVersionTokens[0], subVersionNumber + 1);
                }
                else
                {
                    parsedSpecialVersionToken = specialVersionToken;
                }

                if (!string.IsNullOrWhiteSpace(parsedSpecialVersionToken))
                {
                    parsedVersionBuilder.AppendFormat("-{0}", parsedSpecialVersionToken);
                }
            }

            versionNumberMask = parsedVersionBuilder.ToString();
            return versionNumberMask;
        }

        IEnumerable<string> RequestDeployments(Release release, IEnumerable<DeploymentEnvironment> environments)
        {
            var linksToDeploymentTasks = new List<string>();
            foreach (var environment in environments)
            {
                var deployment = Session.DeployRelease(release, environment, Force);
                var linkToTask = deployment.Link("Task");
                linksToDeploymentTasks.Add(linkToTask);

                Log.InfoFormat("Successfully scheduled release '{0}' for deployment to environment '{1}'" + deployment.Name, release.Version, environment.Name);
            }
            return linksToDeploymentTasks;
        }
    }
}
