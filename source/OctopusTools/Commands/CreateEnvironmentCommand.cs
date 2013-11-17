using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Octopus.Client.Model;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    [Command("create-environment", Description = "Creates a project")]
    public class CreateEnvironmentCommand : ApiCommand
    {
        public CreateEnvironmentCommand(IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(repositoryFactory, log)
        {
        }

        public string EnvironmentName { get; set; }
        public bool IgnoreIfExists { get; set; }

        protected override void SetOptions(OptionSet options)
        {
            options.Add("name=", "The name of the environment", v => EnvironmentName = v);
            options.Add("ignoreIfExists", "If the project already exists, an error will be returned. Set this flag to ignore the error.", v => IgnoreIfExists = true);
        }

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
            env = Repository.Environments.Create(new EnvironmentResource { Name = EnvironmentName });

            Log.Info("Environment created. ID: " + env.Id);
        }
    }
}
