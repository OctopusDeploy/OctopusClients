using System;
using System.Collections.Generic;
using Autofac;
using OctopusTools.Client;
using OctopusTools.Commands;
using OctopusTools.Diagnostics;
using OctopusTools.Infrastructure;

namespace OctopusTools
{
    public class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var container = BuildContainer(args);
                container.Resolve<ICommandProcessor>().Process(args);
                return 0;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                return 1;
            }
        }      

        static IContainer BuildContainer(IEnumerable<string> args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ClientModule(args));
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
