using System;
using Autofac;
using OctopusTools.Client;
using OctopusTools.Commands;
using OctopusTools.Diagnostics;
using OctopusTools.Infrastructure;

namespace OctopusTools
{
    public class Program
    {
        static void Main(string[] args)
        {
            var container = BuildContainer();

            container.Resolve<ICommandProcessor>().Process(args);
        }

        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ClientModule());
            builder.RegisterModule(new CommandModule());
            builder.RegisterModule(new LoggingModule());
            builder.RegisterCommand<HelpCommand>("help", "Prints this help text", "h", "?");
            builder.RegisterCommand<ListEnvironmentsCommand>("list-environments", "List all environments");
            builder.RegisterCommand<CreateReleaseCommand>("create-release", "Creates and (optionally) deploys a release");
            builder.RegisterCommand<DeployReleaseCommand>("deploy-release", "Deploys a release");

            return builder.Build();
        }
    }
}
