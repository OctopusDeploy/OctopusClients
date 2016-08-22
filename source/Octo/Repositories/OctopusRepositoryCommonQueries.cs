using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Repositories
{
    public class OctopusRepositoryCommonQueries
    {
        readonly IOctopusRepository repository;
        readonly ILogger log;

        public OctopusRepositoryCommonQueries(IOctopusRepository repository, ILogger log)
        {
            this.repository = repository;
            this.log = log;
        }

        public ProjectResource GetProjectByName(string projectName)
        {
            log.Debug("Finding project: " + projectName);
            var project = repository.Projects.FindByName(projectName);
            if (project == null)
                throw new CouldNotFindException("a project named", projectName);
            return project;
        }

        public EnvironmentResource GetEnvironmentByName(string environmentName)
        {
            log.Debug("Finding environment: " + environmentName);
            var environment = repository.Environments.FindByName(environmentName);
            if (environment == null)
                throw new CouldNotFindException("an environment named", environmentName);
            return environment;
        }

        public ReleaseResource GetReleaseByVersion(string versionNumber, ProjectResource project, ChannelResource channel)
        {
            string message;
            ReleaseResource releaseToPromote = null;
            if (string.Equals("latest", versionNumber, StringComparison.CurrentCultureIgnoreCase))
            {
                message = channel == null
                    ? "latest release for project"
                    : $"latest release in channel '{channel.Name}'";

                log.Debug($"Finding {message}");

                if (channel == null)
                {
                    releaseToPromote = repository
                        .Projects
                        .GetReleases(project)
                        .Items // We only need the first page
                        .OrderByDescending(r => SemanticVersion.Parse(r.Version))
                        .FirstOrDefault();
                }
                else
                {
                    repository.Projects.GetReleases(project).Paginate(repository, page =>
                    {
                        releaseToPromote = page.Items
                            .OrderByDescending(r => SemanticVersion.Parse(r.Version))
                            .FirstOrDefault(r => r.ChannelId == channel.Id);

                        // If we haven't found one yet, keep paginating
                        return releaseToPromote == null;
                    });
                }
            }
            else
            {
                message = $"release {versionNumber}";
                log.Debug($"Finding {message}");
                releaseToPromote = repository.Projects.GetReleaseByVersion(project, versionNumber);
            }

            if (releaseToPromote == null)
            {
                throw new CouldNotFindException($"the {message}", project.Name);
            }
            return releaseToPromote;
        }

        public IReadOnlyList<TenantResource> FindTenants(IReadOnlyList<string> tenantNames, IReadOnlyList<string> tenantTags)
        {
            if (!tenantNames.Any() && !tenantTags.Any())
            {
                return new List<TenantResource>(0);
            }

            if (!repository.SupportsTenants())
            {
                throw new CommandException(
                    "Your Octopus server does not support tenants, which was introduced in Octopus 3.4. Please upgrade your Octopus server, enable the multi-tenancy feature or remove the --tenant and --tenanttag arguments.");
            }

            var tenantsByName = FindTenantsByName(tenantNames);
            var tenantsByTags = FindTenantsByTags(tenantTags);

            var distinctTenants = tenantsByTags.Concat(tenantsByName)
                .GroupBy(t => t.Id)
                .Select(g => g.First())
                .ToList();

            return distinctTenants;
        }

        private IEnumerable<TenantResource> FindTenantsByName(IReadOnlyList<string> tenantNames)
        {
            if (!tenantNames.Any())
            {
                return Enumerable.Empty<TenantResource>();
            }

            if (tenantNames.Contains("*"))
            {
                return repository.Tenants.FindAll();
            }

            var tenantsByName = repository.Tenants.FindByNames(tenantNames);
            var missing = tenantsByName == null || !tenantsByName.Any()
                ? tenantNames.ToArray()
                : tenantNames.Except(tenantsByName.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToArray();

            var tenantsById = repository.Tenants.Get(missing);

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

        private IEnumerable<TenantResource> FindTenantsByTags(IReadOnlyList<string> tenantTags)
        {
            if (!tenantTags.Any())
            {
                return Enumerable.Empty<TenantResource>();
            }

            var tenantsByTag = repository.Tenants.FindAll(null, tenantTags.ToArray());

            if (!tenantsByTag.Any())
            {
                throw new ArgumentException($"Could not find any tenants matching the tags {string.Join(", ", tenantTags)}");
            }

            return tenantsByTag;
        }
    }
}