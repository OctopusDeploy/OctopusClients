using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("override-autodeploy", Description = "Override the release that auto deploy will use")]
    public class OverrideAutoDeployCommand : ApiCommand
    {
        public OverrideAutoDeployCommand(IOctopusRepositoryFactory repositoryFactory, ILog log,
            IOctopusFileSystem fileSystem) : base(repositoryFactory, log, fileSystem)
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

        protected override void Execute()
        {
            var project = RepositoryCommonQueries.GetProjectByName(ProjectName);
            var release = RepositoryCommonQueries.GetReleaseByVersion(ReleaseVersionNumber, project, null);
            var tenants = FindTenants();
            foreach (var environmentName in EnvironmentNames)
            {
                var environment = RepositoryCommonQueries.GetEnvironmentByName(environmentName);

                if (!tenants.Any())
                {
                    project.AddAutoDeployReleaseOverride(environment, release);
                    Log.Info(
                        $"Auto deploy will deploy version {release.Version} of the project {project.Name} to the environment {environment.Name}");
                }
                else
                {
                    foreach (var tenant in tenants)
                    {
                        if (!tenant.ProjectEnvironments.ContainsKey(project.Id))
                        {
                            Log.Warn(
                                $"The tenant {tenant.Name} was skipped because it has not been connected to the project {project.Name}");
                            continue;
                        }
                        if (!tenant.ProjectEnvironments[project.Id].Contains(environment.Id))
                        {
                            Log.Warn(
                                $"The tenant {tenant.Name} was skipped because it has not been connected to the environment {environment.Name}");
                            continue;
                        }

                        project.AddAutoDeployReleaseOverride(environment, tenant, release);
                        Log.Info(
                            $"Auto deploy will deploy version {release.Version} of the project {project.Name} to the environment {environment.Name} for the tenant {tenant.Name}");
                    }
                }
            }
            Repository.Projects.Modify(project);
        }

        private IReadOnlyList<TenantResource> FindTenants()
        {
            if (!TenantNames.Any() && !TenantTags.Any())
            {
                return new List<TenantResource>(0);
            }

            if (!Repository.SupportsTenants())
            {
                throw new CommandException(
                    "Your Octopus server does not support tenants, which was introduced in Octopus 3.4. Please upgrade your Octopus server, enable the multi-tenancy feature or remove the --tenant and --tenanttag arguments.");
            }

            var tenantsByName = FindTenantsByName();
            var tenantsByTags = FindTenantsByTags();

            var distinctTenants = tenantsByTags.Concat(tenantsByName)
                .GroupBy(t => t.Id)
                .Select(g => g.First())
                .ToList();

            return distinctTenants;
        }

        private IEnumerable<TenantResource> FindTenantsByName()
        {
            if (!TenantNames.Any())
            {
                return Enumerable.Empty<TenantResource>();
            }

            if (TenantNames.Contains("*"))
            {
                return Repository.Tenants.FindAll();
            }

            var tenantsByName = Repository.Tenants.FindByNames(TenantNames);
            var missing = tenantsByName == null || !tenantsByName.Any()
                ? TenantNames.ToArray()
                : TenantNames.Except(tenantsByName.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToArray();

            var tenantsById = Repository.Tenants.Get(missing);

            missing = tenantsById == null || !tenantsById.Any()
                ? missing
                : missing.Except(tenantsById.Select(e => e.Id), StringComparer.OrdinalIgnoreCase).ToArray();

            if (missing.Any())
            {
                throw new ArgumentException($"Could not find the {"tenant" + (missing.Length == 1 ? "" : "s")} {string.Join(", ", missing)} on the Octopus server.");
            }

            var allTenants = Enumerable.Empty<TenantResource>();
            if (tenantsById != null)
            {
                allTenants = allTenants.Concat(tenantsById);
            }
            if (tenantsByName != null)
            {
                allTenants = allTenants.Concat(tenantsByName);
            }

            return allTenants;
        }

        private IEnumerable<TenantResource> FindTenantsByTags()
        {
            if (!TenantTags.Any())
            {
                return Enumerable.Empty<TenantResource>();
            }

            var tenantsByTag = Repository.Tenants.FindAll(null, TenantTags.ToArray());

            if (!tenantsByTag.Any())
            {
                throw new ArgumentException($"Could not find any tenants matching the tags {string.Join(", ", TenantTags)}");
            }

            return tenantsByTag;
        }
    }
}