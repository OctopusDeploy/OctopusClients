using System;
using System.Linq;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Commands
{
	public class PromoteDeployment : ApiCommand
	{
		readonly IDeploymentWatcher _deploymentWatcher;
		readonly IOctopusSessionFactory _client;

		public PromoteDeployment(IOctopusSessionFactory client, ILog log, IDeploymentWatcher deploymentWatcher) : base(client, log)
		{
			_deploymentWatcher = deploymentWatcher;
			_client = client;

			DeploymentStatusCheckSleepCycle = TimeSpan.FromSeconds(10);
			DeploymentTimeout = TimeSpan.FromMinutes(10);
		}

		public string ProjectName { get; set; }
		public string SourceEnvironmentName { get; set; }
		public string DestinationEnvironmentName { get; set; }

		public bool Force { get; set; }
		public bool WaitForDeployment { get; set; }
		public TimeSpan DeploymentTimeout { get; set; }
		public TimeSpan DeploymentStatusCheckSleepCycle { get; set; }

		public override OptionSet Options
		{
			get
			{
				var options = base.Options;
				options.Add("project=", "Name of the project", v => ProjectName = v);
				options.Add("from=", "Name of the source environment", v => SourceEnvironmentName = v);
				options.Add("to=", "Name of the destination environment", v => DestinationEnvironmentName = v);

				options.Add("force", "Whether to force redeployment of already installed packages (flag, default false).", v => Force = true);
				options.Add("waitfordeployment", "Whether to wait synchronously for deployment to finish.", v => WaitForDeployment = true);
				options.Add("deploymenttimeout=", "[Optional] Specifies maximum time (timespan format) that deployment can take (default 00:10:00)", v => DeploymentTimeout = TimeSpan.Parse(v));
				options.Add("deploymentchecksleepcycle=", "[Optional] Specifies how much time (timespan format) should elapse between deployment status checks (default 00:00:10)", v => DeploymentStatusCheckSleepCycle = TimeSpan.Parse(v));
               

				return options;
			}
		}

		public override void Execute()
		{
			if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
			if (string.IsNullOrWhiteSpace(SourceEnvironmentName)) throw new CommandException("Please specify a source environment name using the parameter: --from=EnvironmentSource");
			if (string.IsNullOrWhiteSpace(DestinationEnvironmentName)) throw new CommandException("Please specify an destination environment name using the parameter: --to=EnvironmentDestination");


			Log.Debug("Finding project: " + ProjectName);
			var project = Session.GetProject(ProjectName);

			Log.Debug("Finding environment: " + SourceEnvironmentName);
			var environment = Session.GetEnvironment(SourceEnvironmentName);


			var skip = 0;
			var take = 64;

			Log.Debug("Finding releases for project...");

			bool isDeployedToEnvironment = false;
			SemanticVersion semanticVersion = null;

			while (true)
			{
				var releases = Session.GetReleases(project, skip, take);

				foreach (var release in releases)
				{
					semanticVersion = SemanticVersion.Parse(release.Version);
					var environments = Session.List<Deployment>(release.Link("Deployments"));
					isDeployedToEnvironment |= environments.Any(x => x.EnvironmentId == environment.Id);
					if (isDeployedToEnvironment) break;
				}

				if (isDeployedToEnvironment)
				{
					Log.Debug(string.Format("Calling DeployReleaseCommand with version: {0}", semanticVersion));

					var deployRelease = new DeployReleaseCommand(_client, Log, _deploymentWatcher)
					{
					    ProjectName = ProjectName, 
						Force = Force, 
						VersionNumber = semanticVersion.ToString(), 
						DeploymentStatusCheckSleepCycle = DeploymentStatusCheckSleepCycle, 
						DeploymentTimeout = DeploymentTimeout, 
						WaitForDeployment = WaitForDeployment
					};
					deployRelease.DeployToEnvironmentNames.Add(DestinationEnvironmentName);
					deployRelease.Execute();
					break;
				}

				skip += releases.Count;

				if (releases.Count == 0)
				{
					break;
				}
			}


		}
	}
}
