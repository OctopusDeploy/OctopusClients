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
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    log.Error("Error: " + inner.Message);
                }

                if (args.Any(a => a.TrimStart("/-".ToCharArray()).ToLowerInvariant() == "--debug"))
                {
                    foreach (var inner in ex.InnerExceptions)
                    {
                        log.Debug(inner);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                log.Error("Argument error: ");
                log.Error(ex.Message);
            }
            catch (CommandException ex)
            {
                log.Error("Command error: ");
                log.Error(ex.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                log.Debug(ex);
            }
        }

        static string GetFirstArgument(IEnumerable<string> args)
        {
            return (args.FirstOrDefault() ?? string.Empty).ToLowerInvariant().TrimStart('-', '/');
        }
    }
}