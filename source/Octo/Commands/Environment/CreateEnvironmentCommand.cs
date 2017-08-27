using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands.Environment
{
    [Command("create-environment", Description = "Creates a deployment environment")]
    public class CreateEnvironmentCommand : ApiCommand, ISupportFormattedOutput
    {
        EnvironmentResource env;

        public CreateEnvironmentCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Environment creation");
            options.Add("name=", "The name of the environment", v => EnvironmentName = v);
            options.Add("ignoreIfExists", "If the environment already exists, an error will be returned. Set this flag to ignore the error.", v => IgnoreIfExists = true);
        }

        public string EnvironmentName { get; set; }
        public bool IgnoreIfExists { get; set; }

        public async Task Request()
        {
            if (string.IsNullOrWhiteSpace(EnvironmentName)) throw new CommandException("Please specify an environment name using the parameter: --name=XYZ");
            
            env = await Repository.Environments.FindByName(EnvironmentName).ConfigureAwait(false);
            if (env != null)
            {
                if (IgnoreIfExists)
                {
                    commandOutputProvider.Information("The environment {Environment:l} (ID {Id:l}) already exists", env.Name, env.Id);
                    return;
                }

                throw new CommandException("The environment " + env.Name + " (ID " + env.Id + ") already exists");
            }

            commandOutputProvider.Information("Creating environment: {Environment:l}", EnvironmentName);
            env = await Repository.Environments.Create(new EnvironmentResource {Name = EnvironmentName}).ConfigureAwait(false);
        }
        
        public void PrintDefaultOutput()
        {
            commandOutputProvider.Information("Environment created. ID: {Id:l}", env.Id);
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(new
            {
                env.Id,
                env.Name,
            });
        }
    }
}