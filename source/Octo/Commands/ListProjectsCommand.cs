using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Newtonsoft.Json;
using Octo.Commands;

namespace Octopus.Cli.Commands
{
    [Command("list-projects", Description = "Lists all projects")]
    public class ListProjectsCommand : ApiCommand, ISupportFormattedOutput
    {
        private List<Client.Model.ProjectResource> _projectResources;

        public ListProjectsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, log, fileSystem, commandOutputProvider)
        {
        }

        public async Task Query()
        {
            var projects = await Repository.Projects.FindAll().ConfigureAwait(false);
            _projectResources = projects;
        }

        public void PrintDefaultOutput()
        {
            Log.Information("Projects: {Count}", _projectResources.Count);
            foreach (var project in _projectResources)
            {
                Log.Information(" - {Project:l} (ID: {Id:l})", project.Name, project.Id);
            }
        }

        public void PrintJsonOutput()
        {
            Log.Information(
                JsonConvert.SerializeObject(
                    _projectResources.Select(project => new
                    {
                        project.Id,
                        project.Name
                    }).ToArray(),
                Formatting.Indented));
        }

        public void PrintXmlOutput()
        {
            throw new NotImplementedException();
        }
    }
}