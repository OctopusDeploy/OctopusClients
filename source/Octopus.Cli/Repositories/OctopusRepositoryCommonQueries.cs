using System;
using System.Linq;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Repositories
{
    public class OctopusRepositoryCommonQueries
    {
        readonly IOctopusRepository repository;
        readonly ILog log;

        public OctopusRepositoryCommonQueries(IOctopusRepository repository, ILog log)
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
    }
}