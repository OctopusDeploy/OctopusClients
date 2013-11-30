using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Features.ResolveAnything;
using Octopus.Client.Exceptions;
using OctopusTools.Commands;
using OctopusTools.Diagnostics;
using OctopusTools.Infrastructure;
using log4net;

namespace OctopusTools
{
    public class Program
    {
        static readonly ILog Log = Logger.Default;

        static int Main(string[] args)
        {
            Log.Info("Octopus Deploy Command Line Tool, version " + typeof(Program).Assembly.GetFileVersion());
            Console.Title = "Octopus Deploy Command Line Tool";
            Log.Info(string.Empty);
            
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

        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource(t => typeof(ICommand).IsAssignableFrom(t)));
            builder.RegisterType<CommandLocator>().As<ICommandLocator>();
            builder.RegisterType<PackageVersionResolver>().As<IPackageVersionResolver>();
            builder.RegisterType<OctopusRepositoryFactory>().As<IOctopusRepositoryFactory>();
            builder.RegisterAssemblyTypes(typeof(Program).Assembly).Where(t => typeof(ICommand).IsAssignableFrom(t)).AsSelf().As<ICommand>();
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
                Log.Error(ex);

                foreach (var loaderException in reflex.LoaderExceptions)
                {
                    Log.Error(loaderException);

                    if (!(loaderException is FileNotFoundException)) 
                        continue;

                    var exFileNotFound = loaderException as FileNotFoundException;
                    if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                    {
                        Log.ErrorFormat("Fusion log: {0}", exFileNotFound.FusionLog);
                    }
                }

                return -43;
            }

            var octo = ex as OctopusException;
            if (octo != null)
            {
                Log.Error("Error from Octopus server (HTTP " + octo.HttpStatusCode + "): " + octo.Message);
                return -7;
            }

            Log.Error(ex);
            return -3;
        }
    }
}
