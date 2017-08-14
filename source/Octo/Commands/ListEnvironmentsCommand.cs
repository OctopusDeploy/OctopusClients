using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Octo.Commands;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("list-environments", Description = "List environments")]
    public class ListEnvironmentsCommand : ApiCommand, ISupportFormattedOutput
    {
        List<EnvironmentResource> environments;

        public ListEnvironmentsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, log, fileSystem, commandOutputProvider)
        {
        }

        public async Task Query()
        {
            environments = await Repository.Environments.FindAll().ConfigureAwait(false);
        }

        public void PrintDefaultOutput()
        {
            Log.Information("Environments: {Count}", environments.Count);

            foreach (var environment in environments)
            {
                Log.Information(" - {Environment:l} (ID: {Id:l})", environment.Name, environment.Id);
            }
        }

        public void PrintJsonOutput()
        {
            Log.Information(
                JsonConvert.SerializeObject(
                    environments.Select(environment => new
                    {
                        environment.Id,
                        environment.Name
                    }).ToArray(),
                    Formatting.Indented));
        }

        public void PrintXmlOutput()
        {
            throw new NotImplementedException();
        }
    }
}