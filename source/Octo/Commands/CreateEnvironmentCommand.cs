using System;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("create-environment", Description = "Creates a deployment environment")]
    public class CreateEnvironmentCommand : ApiCommand
    {
        public CreateEnvironmentCommand(IOctopusRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Environment creation");
            options.Add("name=", "The name of the environment", v => EnvironmentName = v);
            options.Add("ignoreIfExists", "If the environment already exists, an error will be returned. Set this flag to ignore the error.", v => IgnoreIfExists = true);
        }

        public string EnvironmentName { get; set; }
        public bool IgnoreIfExists { get; set; }

        protected override async Task Execute()
        {
            if (string.IsNullOrWhiteSpace(EnvironmentName)) throw new CommandException("Please specify an environment name using the parameter: --name=XYZ");

            var env = await Repository.Environments.FindByName(EnvironmentName).ConfigureAwait(false);
            if (env != null)
            {
                if (IgnoreIfExists)
                {
                    Log.Information("The environment " + env.Name + " (ID " + env.Id + ") already exists");
                    return;
                }

                throw new CommandException("The environment " + env.Name + " (ID " + env.Id + ") already exists");
            }

            Log.Information("Creating environment: " + EnvironmentName);
            env = await Repository.Environments.Create(new EnvironmentResource {Name = EnvironmentName}).ConfigureAwait(false);

            Log.Information("Environment created. ID: " + env.Id);
        }
    }
}