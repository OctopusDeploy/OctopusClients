using System;
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
    public class DeleteAutoDeployOverrideCommand : ApiCommand, ISupportFormattedOutput
    {
        IReadOnlyList<TenantResource> tenants;
        ProjectResource project;
        private List<Tuple<EnvironmentResource, TenantResource, DeletedOutcome>> deletedDeplomentOverrides;

        public DeleteAutoDeployOverrideCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory octopusClientFactory, ICommandOutputProvider commandOutputProvider) :
            base(octopusClientFactory, repositoryFactory, fileSystem, commandOutputProvider)
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

            deletedDeplomentOverrides = new List<Tuple<EnvironmentResource, TenantResource, DeletedOutcome>>();
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

        public async Task Request()
        {
            Task<ProjectResource> projectTask = RepositoryCommonQueries.GetProjectByName(ProjectName);
            
            tenants = await RepositoryCommonQueries.FindTenants(TenantNames, TenantTags).ConfigureAwait(false);
            project = await projectTask.ConfigureAwait(false);

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
                deletedDeplomentOverrides.Add(
                    new Tuple<EnvironmentResource, TenantResource, DeletedOutcome>(environment, null, DeletedOutcome.NotFound));
                commandOutputProvider.Warning("Did not find an auto deploy override for the project {Project:l} and environment {Environment:l}", project.Name, environment.Name);
            }
            else
            {
                deletedDeplomentOverrides.Add(
                    new Tuple<EnvironmentResource, TenantResource, DeletedOutcome>(environment, null, DeletedOutcome.Deleted));
                project.AutoDeployReleaseOverrides.Remove(autoDeployOverride);
                commandOutputProvider.Information("Deleted auto deploy release override for the project {Project:l} to the environment {Environment:l}", project.Name, environment.Name);
            }
        }

        private void DeleteOverrideForTenant(ProjectResource project, EnvironmentResource environment, TenantResource tenant)
        {
            var autoDeployOverride = project.AutoDeployReleaseOverrides.SingleOrDefault(
                o => o.EnvironmentId == environment.Id && o.TenantId == tenant.Id);

            if (autoDeployOverride == null)
            {
                deletedDeplomentOverrides.Add(
                    new Tuple<EnvironmentResource, TenantResource, DeletedOutcome>(environment, tenant, DeletedOutcome.NotFound));
                commandOutputProvider.Warning("Did not find an auto deploy override for the project {Project:l}, environment {Environment:l} and tenant {Tenant:l}", project.Name, environment.Name, tenant.Name);
            }
            else
            {
                deletedDeplomentOverrides.Add(
                    new Tuple<EnvironmentResource, TenantResource, DeletedOutcome>(environment, tenant, DeletedOutcome.Deleted));
                project.AutoDeployReleaseOverrides.Remove(autoDeployOverride);
                commandOutputProvider.Information("Deleted auto deploy release override for the project {Project:l} to the environment {Environment:l} and tenant {Tenant:l}", project.Name, environment.Name, tenant.Name);
            }
        }

        public void PrintDefaultOutput()
        {
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(new
            {
                Project = new {project.Id, project.Name},
                AutoDeployOverridesRemoved = deletedDeplomentOverrides.Select(x => new
                {
                    Environment = new {x.Item1.Id, x.Item1.Name},
                    Tenant = x.Item2 == null ? null : new {x.Item2.Id, x.Item2.Name},
                    Outcome = x.Item3.ToString()
                })
            });
        }

        private enum DeletedOutcome
        {
            Deleted,
            NotFound
        }
    }
}