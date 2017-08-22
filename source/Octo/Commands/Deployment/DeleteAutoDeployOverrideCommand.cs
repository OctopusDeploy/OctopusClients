using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands.Deployment
{
    [Command("delete-autodeployoverride", Description = "Delete auto deploy release overrides")]
    public class DeleteAutoDeployOverrideCommand : ApiCommand
    {
        public DeleteAutoDeployOverrideCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory octopusClientFactory, ICommandOutputProvider commandOutputProvider) :
            base(octopusClientFactory, repositoryFactory, log, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Delete auto deploy release override");
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("environment=",
                "Name of an environment the override will apply to. Specify this argument multiple times to add multiple environments.",
                v => EnvironmentNames.Add(v));
            options.Add("tenant=",
                "[Optional] Name of a tenant the override will apply to. Specify this argument multiple times to add multiple tenants or use `*` wildcard for all tenants.",
                t => TenantNames.Add(t));
            options.Add("tenanttag=",
                "[Optional] A tenant tag used to match tenants that the override will apply to. Specify this argument multiple times to add multiple tenant tags",
                tt => TenantTags.Add(tt));
        }

        public string ProjectName { get; set; }
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

            if (!EnvironmentNames.Any())
            {
                throw new CommandException(
                    "Please specify an environment using the environment parameter: --environment=MyEnvironment");
            }
        }

        protected override async Task Execute()
        {
            var projectTask = RepositoryCommonQueries.GetProjectByName(ProjectName);
            var tenants = await RepositoryCommonQueries.FindTenants(TenantNames, TenantTags).ConfigureAwait(false);
            var project = await projectTask.ConfigureAwait(false);

            foreach (var environmentName in EnvironmentNames)
            {
                var environment = await RepositoryCommonQueries.GetEnvironmentByName(environmentName).ConfigureAwait(false);

                if (!tenants.Any())
                {
                    DeleteOverrideForEnvironment(project, environment);
                }
                else
                {
                    foreach (var tenant in tenants)
                    {
                        DeleteOverrideForTenant(project, environment, tenant);
                    }
                }
            }
            await Repository.Projects.Modify(project).ConfigureAwait(false);
        }

        private void DeleteOverrideForEnvironment(ProjectResource project, EnvironmentResource environment)
        {
            var autoDeployOverride = project.AutoDeployReleaseOverrides.SingleOrDefault(
                o => o.EnvironmentId == environment.Id && o.TenantId == null);

            if (autoDeployOverride == null)
            {
                Log.Warning("Did not find an auto deploy override for the project {Project:l} and environment {Environment:l}", project.Name, environment.Name);
            }
            else
            {
                project.AutoDeployReleaseOverrides.Remove(autoDeployOverride);
                Log.Information("Deleted auto deploy release override for the project {Project:l} to the environment {Environment:l}", project.Name, environment.Name);
            }
        }

        private void DeleteOverrideForTenant(ProjectResource project, EnvironmentResource environment, TenantResource tenant)
        {
            var autoDeployOverride = project.AutoDeployReleaseOverrides.SingleOrDefault(
                o => o.EnvironmentId == environment.Id && o.TenantId == tenant.Id);

            if (autoDeployOverride == null)
            {
                Log.Warning("Did not find an auto deploy override for the project {Project:l}, environment {Environment:l} and tenant {Tenant:l}", project.Name, environment.Name, tenant.Name);
            }
            else
            {
                project.AutoDeployReleaseOverrides.Remove(autoDeployOverride);
                Log.Information("Deleted auto deploy release override for the project {Project:l} to the environment {Environment:l} and tenant {Tenant:l}", project.Name, environment.Name, tenant.Name);
            }
        }
    }
}