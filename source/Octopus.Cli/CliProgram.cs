using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Autofac;
using Octopus.Cli.Commands.Releases;
using Octopus.Cli.Diagnostics;
using Octopus.Cli.Exporters;
using Octopus.Cli.Importers;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Exceptions;
using Serilog;

namespace Octopus.Cli
{
    public class CliProgram
    {
        public int Execute(string[] args)
        {
#if !HTTP_CLIENT_SUPPORTS_SSL_OPTIONS
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12;
#endif
            ConfigureLogger();
            return Run(args);
        }

        internal int Run(string[] args)
        {

            Console.Title = "Octopus Deploy Command Line Tool";
            
            try
            {
                var container = BuildContainer();
                var commandLocator = container.Resolve<ICommandLocator>();
                var first = GetFirstArgument(args);
                var command = GetCommand(first, commandLocator);
                command.Execute(args.Skip(1).ToArray()).GetAwaiter().GetResult();
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
               .MinimumLevel.ControlledBy(LogUtilities.LevelSwitch)
               .WriteTo.Trace()
               .WriteTo.ColoredConsole(outputTemplate: "{Message}{NewLine}{Exception}")
               .CreateLogger();
        }

        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            var thisAssembly = typeof (CliProgram).GetTypeInfo().Assembly;

            builder.RegisterModule(new LoggingModule());

            builder.RegisterAssemblyTypes(thisAssembly).As<ICommand>().AsSelf();
            builder.RegisterType<CommandLocator>().As<ICommandLocator>();

            builder.RegisterType<CommandOutputProvider>().As<ICommandOutputProvider>().SingleInstance();

            builder.RegisterAssemblyTypes(thisAssembly).As<IExporter>().AsSelf();
            builder.RegisterAssemblyTypes(thisAssembly).As<IImporter>().AsSelf();
            builder.RegisterType<ExporterLocator>().As<IExporterLocator>();
            builder.RegisterType<ImporterLocator>().As<IImporterLocator>();

            builder.RegisterType<ReleasePlanBuilder>().As<IReleasePlanBuilder>().SingleInstance();
            builder.RegisterType<PackageVersionResolver>().As<IPackageVersionResolver>().SingleInstance();
            builder.RegisterType<ChannelVersionRuleTester>().As<IChannelVersionRuleTester>().SingleInstance();

            builder.RegisterType<OctopusClientFactory>().As<IOctopusClientFactory>();
            builder.RegisterType<OctopusRepositoryFactory>().As<IOctopusAsyncRepositoryFactory>();

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
                if (LogExtensions.IsKnownEnvironment())
                {
                    Log.Error("This error is most likely ocurring while executing Octo.exe as part of an automated build process. The following doc is recommended to get some tips on how to troubleshoot this: https://g.octopushq.com/OctoexeTroubleshooting");
                }
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
                Log.Information("{HttpErrorMessage:l}", octo.Message);
                Log.Error("Error from Octopus server (HTTP {StatusCode} {StatusDescription})", octo.HttpStatusCode, (HttpStatusCode) octo.HttpStatusCode);
                return -7;
            }

            Log.Error(ex, "");
            return -3;
        }
    }
}