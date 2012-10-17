using System;
using System.Collections.Generic;
using System.Linq;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Commands
{
	public class ListLatestDeploymentsCommand : ApiCommand
	{
		public string ProjectName { get; set; }
		public string EnvironmentName { get; set; }

		public ListLatestDeploymentsCommand(IOctopusSessionFactory session, ILog log)
			: base(session, log)
		{
		}
		
		public override OptionSet Options
		{
			get
			{
				var options = base.Options;
				options.Add("project=", "Name of the project", v => ProjectName = v);
				options.Add("environment=", "Name of the environment", v => EnvironmentName = v);
				return options;
			}
		}
		
		public override void Execute()
		{
			if(string.IsNullOrWhiteSpace(ProjectName))
			{
				throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
			}
			
			Log.Debug("Finding project: " + ProjectName);
			var project = Session.GetProject(ProjectName);
			Log.Debug("Finding most recent deployments...");
			var deployments = Session.GetMostRecentDeployments(project);

			var environmentsById = (Session.ListEnvironments()).ToDictionary(e => e.Id, e => e);
			
			foreach(var deployment in deployments)
			{
				var nameOfDeploymentEnvironment = environmentsById[deployment.EnvironmentId].Name;
				if (EnvironmentName != null && !string.Equals(EnvironmentName, nameOfDeploymentEnvironment, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}

				var task = Session.Get<Task>(deployment.Link("Task"));
				var release = Session.GetReleaseById(project, deployment.ReleaseId);
				
				var propertiesToLog = new List<string>();
				propertiesToLog.AddRange(FormatTaskPropertiesAsStrings(task));
				propertiesToLog.AddRange(FormatReleasePropertiesAsStrings(release));

				Log.InfoFormat(" - Environment: {0}", nameOfDeploymentEnvironment);
				foreach(var property in propertiesToLog)
				{
					Log.InfoFormat("   {0}", property);
				}
				Log.InfoFormat("");
			}
		}
		
		private static IEnumerable<string> FormatTaskPropertiesAsStrings(Task task)
		{
			return new List<string>{
				"Date: " + task.QueueTime,
				"Duration: " + task.Duration.ToString(@"hh\:mm\:ss"),
				"State: "+task.State                  
			};
		}
		
		private static IEnumerable<string> FormatReleasePropertiesAsStrings(Release release)
		{
			return new List<string>{
				"Version: " + release.Version,
				"Assembled: "+release.Assembled,
				"Package Versions: " + GetPackageVersionsAsString(release.PackageVersions),
				"Release Notes: " + ((release.ReleaseNotes != null) ? release.ReleaseNotes.Replace(System.Environment.NewLine,@"\n"): "")
			};
		}
		
		private static string GetPackageVersionsAsString(IEnumerable<PackageVersion> packageVersions)
		{
			var packageVersionsAsString = "";
			
			foreach(var pv in packageVersions)
			{
				var packageVersionAsString = pv.Id + " " + pv.Version;

				if (packageVersionsAsString.Contains(packageVersionAsString))
				{
					continue;
				}
				if(!String.IsNullOrEmpty(packageVersionsAsString))
				{
					packageVersionsAsString += "; ";
				}
				packageVersionsAsString += packageVersionAsString;
			}
			return packageVersionsAsString;
		}
	}
}
