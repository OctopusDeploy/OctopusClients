//using System;
//using System.Collections.Generic;
//using System.Linq;
//using OctopusTools.Infrastructure;
//using OctopusTools.Model;
//using log4net;

//namespace OctopusTools.Commands
//{
//    [Command("list-latestdeployments", Description = "List the releases last-deployed in each environment")]
//    public class ListLatestDeploymentsCommand : ApiCommand
//    {
//        readonly HashSet<string> environments = new HashSet<string>(StringComparer.OrdinalIgnoreCase); 
//        readonly HashSet<string> projects = new HashSet<string>(StringComparer.OrdinalIgnoreCase); 

//        public ListLatestDeploymentsCommand(IOctopusRepositoryFactory repositoryFactory, ILog log) : base(repositoryFactory, log)
//        {
//        }

//        protected override void SetOptions(OptionSet options)
//        {
//            options.Add("project=", "Name of a project to filter by. Can be specified many times.", v => projects.Add(v));
//            options.Add("environment=", "Name of an environment to filter by. Can be specified many times.", v => environments.Add(v));
//        }

//        protected override void Execute()
//        {
//            var projectsFilter = new string[0];
//            if (projects.Count > 0)
//            {
//                Log.Debug("Loading projects...");
//                projectsFilter = Repository.Projects.FindByNames(projects).Select(p => p.Id).ToArray();
//            }
            
//            var environmentsFilter = new string[0];
//            if (environments.Count > 0)
//            {
//                Log.Debug("Loading environments...");
//                environmentsFilter = Repository.Environments.FindByNames(environments).Select(p => p.Id).ToArray();
//            }

//            //var projects = Repository.Projects

//            Log.Debug("Finding most recent deployments...");

//            var deployments = Repository.Deployments.FindAll(projectsFilter, environmentsFilter);

//            foreach (var deployment in deployments)
//            {
//                var nameOfDeploymentEnvironment = environmentsById[deployment.EnvironmentId].Name;
//                if (EnvironmentName != null && !string.Equals(EnvironmentName, nameOfDeploymentEnvironment, StringComparison.InvariantCultureIgnoreCase))
//                {
//                    continue;
//                }

//                var task = Session.Get<Task>(deployment.Link("Task"));
//                var release = Session.Get<Release>(deployment.Link("Release"));

//                var propertiesToLog = new List<string>();
//                propertiesToLog.AddRange(FormatTaskPropertiesAsStrings(task));
//                propertiesToLog.AddRange(FormatReleasePropertiesAsStrings(release));

//                Log.InfoFormat(" - Environment: {0}", nameOfDeploymentEnvironment);
//                foreach (var property in propertiesToLog)
//                {
//                    Log.InfoFormat("   {0}", property);
//                }
//                Log.InfoFormat("");
//            }
//        }

//        private static IEnumerable<string> FormatTaskPropertiesAsStrings(Task task)
//        {
//            return new List<string>{
//                "Date: " + task.QueueTime,
//                "Duration: " + task.Duration,
//                "State: "+task.State                  
//            };
//        }

//        private static IEnumerable<string> FormatReleasePropertiesAsStrings(Release release)
//        {
//            return new List<string>{
//                "Version: " + release.Version,
//                "Assembled: "+release.Assembled,
//                "Package Versions: " + GetPackageVersionsAsString(release.PackageVersions),
//                "Release Notes: " + ((release.ReleaseNotes != null) ? release.ReleaseNotes.Replace(System.Environment.NewLine,@"\n"): "")
//            };
//        }

//        private static string GetPackageVersionsAsString(IEnumerable<PackageVersion> packageVersions)
//        {
//            var packageVersionsAsString = "";

//            foreach (var pv in packageVersions)
//            {
//                var packageVersionAsString = pv.Id + " " + pv.Version;

//                if (packageVersionsAsString.Contains(packageVersionAsString))
//                {
//                    continue;
//                }
//                if (!String.IsNullOrEmpty(packageVersionsAsString))
//                {
//                    packageVersionsAsString += "; ";
//                }
//                packageVersionsAsString += packageVersionAsString;
//            }
//            return packageVersionsAsString;
//        }
//    }
//}
