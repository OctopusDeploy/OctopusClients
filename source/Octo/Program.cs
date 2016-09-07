using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Serilog;
using Octopus.Cli.Commands;
using Octopus.Cli.Diagnostics;
using Octopus.Cli.Exporters;
using Octopus.Cli.Importers;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client.Exceptions;
using System.Net;
using Octopus.Cli.Repositories;

namespace Octopus.Cli
{
    public class Program
    {
        static int Main(string[] args)
        {
            ConfigureLogger();
            Log.Information("Octopus Deploy Command Line Tool, version " + typeof (Program).GetInformationalVersion());
            Console.Title = "Octopus Deploy Command Line Tool";
            Log.Information(string.Empty);

            try
            {
                var container = BuildContainer();
                var commandLocator = container.Resolve<ICommandLocator>();
                var first = GetFirstArgument(args);
                var command = GetCommand(first, commandLocator);
                command.Execute(args.Skip(1).ToArray());
                return 0;
            }
            catch (Exception exception)
            {
                var exit = PrintError(exception);
                Console.WriteLine("Exit code: " + exit);
                return exit;
            }
        }

        public static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Trace()
               .WriteTo.ColoredConsole(outputTemplate: "{Message}{NewLine}{Exception}")
               .CreateLogger();
        }

        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            var thisAssembly = typeof (Program).GetTypeInfo().Assembly;

            builder.RegisterModule(new LoggingModule());

            builder.RegisterAssemblyTypes(thisAssembly).As<ICommand>().AsSelf();
            builder.RegisterType<CommandLocator>().As<ICommandLocator>();

            builder.RegisterAssemblyTypes(thisAssembly).As<IExporter>().AsSelf();
            builder.RegisterAssemblyTypes(thisAssembly).As<IImporter>().AsSelf();
            builder.RegisterType<ExporterLocator>().As<IExporterLocator>();
            builder.RegisterType<ImporterLocator>().As<IImporterLocator>();

            builder.RegisterType<ReleasePlanBuilder>().As<IReleasePlanBuilder>().SingleInstance();
            builder.RegisterType<PackageVersionResolver>().As<IPackageVersionResolver>().SingleInstance();
            builder.RegisterType<ChannelVersionRuleTester>().As<IChannelVersionRuleTester>().SingleInstance();

            builder.RegisterType<OctopusRepositoryFactory>().As<IOctopusRepositoryFactory>();

            builder.RegisterType<OctopusPhysicalFileSystem>().As<IOctopusFileSystem>();

            return builder.Build();
        }

        static ICommand GetCommand(string first, ICommandLocator commandLocator)
        {
            if (string.IsNullOrWhiteSpace(first))
            {
                return commandLocator.Find("help");
            }

            var command = commandLocator.Find(first);
            if (command == null)
                throw new CommandException("Error: Unrecognized command '" + first + "'");

            return command;
        }

        static string GetFirstArgument(IEnumerable<string> args)
        {
            return (args.FirstOrDefault() ?? string.Empty).ToLowerInvariant().TrimStart('-', '/');
        }

        static int PrintError(Exception ex)
        {
            var agg = ex as AggregateException;
            if (agg != null)
            {
                var errors = new HashSet<Exception>(agg.InnerExceptions);
                if (agg.InnerException != null)
                    errors.Add(ex.InnerException);

                var lastExit = 0;
                foreach (var inner in errors)
                {
                    lastExit = PrintError(inner);
                }

                return lastExit;
            }

            var cmd = ex as CommandException;
            if (cmd != null)
            {
                Log.Error(ex.Message);
                return -1;
            }
            var reflex = ex as ReflectionTypeLoadException;
            if (reflex != null)
            {
                Log.Error(ex, "");

                foreach (var loaderException in reflex.LoaderExceptions)
                {
                    Log.Error(loaderException, "");
                }

                return -43;
            }

            var octo = ex as OctopusException;
            if (octo != null)
            {
                Log.Error("Error from Octopus server (HTTP " + octo.HttpStatusCode + "): " + octo.Message);
                return -7;
            }

            Log.Error(ex, "");
            return -3;
        }
    }
}