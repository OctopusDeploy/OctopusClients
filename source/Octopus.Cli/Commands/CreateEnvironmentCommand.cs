using System;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("create-environment", Description = "Creates a deployment environment")]
    public class CreateEnvironmentCommand : ApiCommand
    {
        public CreateEnvironmentCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Environment creation");
            options.Add("name=", "The name of the environment", v => EnvironmentName = v);
            options.Add("ignoreIfExists", "If the environment already exists, an error will be returned. Set this flag to ignore the error.", v => IgnoreIfExists = true);
        }

        public string EnvironmentName { get; set; }
        public bool IgnoreIfExists { get; set; }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(EnvironmentName)) throw new CommandException("Please specify an environment name using the parameter: --name=XYZ");

            var env = Repository.Environments.FindByName(EnvironmentName);
            if (env != null)
            {
                if (IgnoreIfExists)
                {
                    Log.Info("The environment " + env.Name + " (ID " + env.Id + ") already exists");
                    return;
                }

                throw new CommandException("The environment " + env.Name + " (ID " + env.Id + ") already exists");
            }

            Log.Info("Creating environment: " + EnvironmentName);
            env = Repository.Environments.Create(new EnvironmentResource {Name = EnvironmentName});

            Log.Info("Environment created. ID: " + env.Id);
        }
    }
}