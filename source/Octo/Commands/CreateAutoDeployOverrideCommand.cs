using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("create-autodeployoverride", Description = "Override the release that auto deploy will use")]
    public class CreateAutoDeployOverrideCommand : ApiCommand
    {
        public CreateAutoDeployOverrideCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory) :
            base(clientFactory, repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Auto deploy release override");
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("environment=",
                "Name of an environment the override will apply to. Specify this argument multiple times to add multiple environments.",
                v => EnvironmentNames.Add(v));
            options.Add("version=|releaseNumber=", "Release number to use for auto deployments.",
                v => ReleaseVersionNumber = v);
            options.Add("tenant=",
                "[Optional] Name of a tenant the override will apply to. Specify this argument multiple times to add multiple tenants or use `*` wildcard for all tenants.",
                t => TenantNames.Add(t));
            options.Add("tenanttag=",
                "[Optional] A tenant tag used to match tenants that the override will apply to. Specify this argument multiple times to add multiple tenant tags",
                tt => TenantTags.Add(tt));
        }

        public string ProjectName { get; set; }
        public string ReleaseVersionNumber { get; set; }
        public List<string> EnvironmentNames { get; set; } = new List<string>();
        public List<string> TenantNames { get; set; } = new List<string>();
        public List<string> TenantTags { get; set; } = new List<string>();

        protected override void ValidateParameters()
        {
            base.ValidateParameters();

            if (string.IsNullOrEmpty(ProjectName))
            {
                throw new CommandException("Please specify a project using the project parameter: --project=MyProject");
            }

            if (string.IsNullOrEmpty(ReleaseVersionNumber))
            {
                throw new CommandException(
                    "Please specify a release version number using the version parameter: --version=1.0.5");
            }

            if (!EnvironmentNames.Any())
            {
                throw new CommandException(
                    "Please specify an environment using the environment parameter: --environment=MyEnvironment");
            }
        }

        protected override async Task Execute()
        {
            var project = await RepositoryCommonQueries.GetProjectByName(ProjectName).ConfigureAwait(false);
            var releaseTask = RepositoryCommonQueries.GetReleaseByVersion(ReleaseVersionNumber, project, null).ConfigureAwait(false);
            var tenants = await RepositoryCommonQueries.FindTenants(TenantNames, TenantTags).ConfigureAwait(false);
            var release = await releaseTask;

            foreach (var environmentName in EnvironmentNames)
            {
                var environment = await RepositoryCommonQueries.GetEnvironmentByName(environmentName).ConfigureAwait(false);

                if (!tenants.Any())
                {
                    AddOverrideForEnvironment(project, environment, release);
                }
                else
                {
                    foreach (var tenant in tenants)
                    {
                        AddOverrideForTenant(project, environment, tenant, release);
                    }
                }
            }
            await Repository.Projects.Modify(project).ConfigureAwait(false);
        }

        void AddOverrideForEnvironment(ProjectResource project, EnvironmentResource environment, ReleaseResource release)
        {
            project.AddAutoDeployReleaseOverride(environment, release);
            Log.Information($"Auto deploy will deploy version {release.Version} of the project {project.Name} to the environment {environment.Name}");
        }

        void AddOverrideForTenant(ProjectResource project, EnvironmentResource environment, TenantResource tenant, ReleaseResource release)
        {
            if (!tenant.ProjectEnvironments.ContainsKey(project.Id))
            {
                Log.Warning($"The tenant {tenant.Name} was skipped because it has not been connected to the project {project.Name}");
                return;
            }
            if (!tenant.ProjectEnvironments[project.Id].Contains(environment.Id))
            {
                Log.Warning($"The tenant {tenant.Name} was skipped because it has not been connected to the environment {environment.Name}");
                return;
            }

            project.AddAutoDeployReleaseOverride(environment, tenant, release);
            Log.Information($"Auto deploy will deploy version {release.Version} of the project {project.Name} to the environment {environment.Name} for the tenant {tenant.Name}");
        }
    }
}