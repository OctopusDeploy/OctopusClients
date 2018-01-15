using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands.Deployment
{
    [Command("create-autodeployoverride", Description = "Override the release that auto deploy will use")]
    public class CreateAutoDeployOverrideCommand : ApiCommand, ISupportFormattedOutput
    {
        EnvironmentResource environment;
        ProjectResource project;
        ConfiguredTaskAwaitable<ReleaseResource> releaseTask;
        IReadOnlyList<TenantResource> tenants;
        ReleaseResource release;
        private List<Tuple<EnvironmentResource, TenantResource, CreatedOutcome>> createdDeploymentOverides;

        public CreateAutoDeployOverrideCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider) :
            base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
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

            createdDeploymentOverides = new List<Tuple<EnvironmentResource, TenantResource, CreatedOutcome>>();
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

        public async Task Request()
        {
            
            project = await RepositoryCommonQueries.GetProjectByName(ProjectName).ConfigureAwait(false);
            releaseTask = RepositoryCommonQueries.GetReleaseByVersion(ReleaseVersionNumber, project, null).ConfigureAwait(false);
            tenants = await RepositoryCommonQueries.FindTenants(TenantNames, TenantTags).ConfigureAwait(false);
            release = await releaseTask;

            foreach (var environmentName in EnvironmentNames)
            {
                environment = await RepositoryCommonQueries.GetEnvironmentByName(environmentName).ConfigureAwait(false);

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
            createdDeploymentOverides.Add(new Tuple<EnvironmentResource, TenantResource, CreatedOutcome>(environment, null, CreatedOutcome.Created));
            project.AddAutoDeployReleaseOverride(environment, release);
            commandOutputProvider.Information("Auto deploy will deploy version {Version:l} of the project {Project:l} to the environment {Environment:l}", release.Version, project.Name, environment.Name);
        }

        void AddOverrideForTenant(ProjectResource project, EnvironmentResource environment, TenantResource tenant, ReleaseResource release)
        {
            if (!tenant.ProjectEnvironments.ContainsKey(project.Id))
            {
                createdDeploymentOverides.Add(new Tuple<EnvironmentResource, TenantResource, CreatedOutcome>(environment,tenant, CreatedOutcome.NotConnectedToProject));
                commandOutputProvider.Warning("The tenant {Tenant:l} was skipped because it has not been connected to the project {Project:l}", tenant.Name, project.Name);
                return;
            }
            if (!tenant.ProjectEnvironments[project.Id].Contains(environment.Id))
            {
                createdDeploymentOverides.Add(new Tuple<EnvironmentResource, TenantResource, CreatedOutcome>(environment, tenant, CreatedOutcome.NotConnectedToEnvironment));
                commandOutputProvider.Warning("The tenant {Tenant:l} was skipped because it has not been connected to the environment {Environment:l}", tenant.Name, environment.Name);
                return;
            }

            createdDeploymentOverides.Add(new Tuple<EnvironmentResource, TenantResource, CreatedOutcome>(environment, tenant, CreatedOutcome.Created));
            project.AddAutoDeployReleaseOverride(environment, tenant, release);
            commandOutputProvider.Information("Auto deploy will deploy version {Version:l} of the project {Project:l} to the environment {Environment:l} for the tenant {Tenant:l}", release.Version, project.Name, environment.Name, tenant.Name);
        }

        public void PrintDefaultOutput()
        {
            
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(new
            {
                Project = new {project.Id, project.Name},
                AutoDeployVersion = release.Version,
                AutoDeployOveridesCreated = createdDeploymentOverides.Select(x => new
                {
                    Environment = new {x.Item1.Id, x.Item1.Name},
                    Tenant = x.Item2 == null ? null : new {x.Item2.Id, x.Item2.Name},
                    Outcome = x.Item3.ToString()
                })
            });
        }

        private enum CreatedOutcome
        {
            Created,
            NotConnectedToProject,
            NotConnectedToEnvironment
        }

    }
    
}