using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace OctopusTools.Infrastructure
{
    public class CommandProcessor : ICommandProcessor
    {
        readonly ICommandLocator commandLocator;
        readonly ILog log;

        public CommandProcessor(ICommandLocator commandLocator, ILog log)
        {
            this.commandLocator = commandLocator;
            this.log = log;
        }

        public void Process(string[] args)
        {
            var first = GetFirstArgument(args);

            var command =
                commandLocator.Find(first) ??
                commandLocator.Find("help");

            if (command == null)
            {
                throw new InvalidOperationException(string.Format("The command '{0}' is not supported and no help exists.", first));
            }

            log.Info("Octopus Command Line Tool, version " + GetType().Assembly.GetFileVersion());
            log.Info(string.Empty);

            args = args.Skip(1).ToArray();

            try
            {
                var options = command.Options;
                options.Parse(args);

                command.Execute();
            }
            catch (Exception ex)
            {
                PrintError(ex);
            }
        }

        void PrintError(Exception ex)
        {
            var agg = ex as AggregateException;
            if (agg != null)
            {
                var errors = new HashSet<Exception>(agg.InnerExceptions);
                errors.Add(ex.InnerException);
                foreach (var inner in errors)
                {
                    PrintError(inner);
                }

                return;
            }

            var cmd = ex as CommandException;
            if (cmd != null)
            {
                log.Error("Command error: ");
                log.Error(ex.Message);
                return;
            }

            var arg = ex as ArgumentException;
            if (arg != null)
            {
                log.Error("Argument error: ");
                log.Error(ex.Message);
                return;   
            }

            log.Error(ex.Message);
            log.Debug(ex);
        }

        static string GetFirstArgument(IEnumerable<string> args)
        {
            return (args.FirstOrDefault() ?? string.Empty).ToLowerInvariant().TrimStart('-', '/');
        }
    }
}