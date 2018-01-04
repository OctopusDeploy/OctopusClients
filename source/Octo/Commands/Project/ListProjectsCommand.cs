using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Serilog;

namespace Octopus.Cli.Commands.Project
{
    [Command("list-projects", Description = "Lists all projects")]
    public class ListProjectsCommand : ApiCommand, ISupportFormattedOutput
    {
        private List<Octopus.Client.Model.ProjectResource> _projectResources;

        public ListProjectsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
        }

        public async Task Request()
        {
            var projects = await Repository.Projects.FindAll().ConfigureAwait(false);
            _projectResources = projects;
        }

        public void PrintDefaultOutput()
        {
            commandOutputProvider.Information("Projects: {Count}", _projectResources.Count);
            foreach (var project in _projectResources)
            {
                commandOutputProvider.Information(" - {Project:l} (ID: {Id:l})", project.Name, project.Id);
            }
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(
                _projectResources.Select(project => new
                {
                    project.Id,
                    project.Name
                }));
        }
    }
}