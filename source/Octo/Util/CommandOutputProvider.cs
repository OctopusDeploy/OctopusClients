using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Octopus.Cli.Infrastructure;
using Serilog;

namespace Octopus.Cli.Util
{
    public class CommandOutputProvider : ICommandOutputProvider
    {
        private readonly ILogger logger;

        public CommandOutputProvider(ILogger logger)
        {
            this.logger = logger;
        }

        public bool PrintMessages { get; set; }

        public void PrintHeader()
        {
            if (PrintMessages)
            {
                logger.Information("Octopus Deploy Command Line Tool, version {Version:l}",
                        typeof(Program).GetInformationalVersion());
                logger.Information(string.Empty);
            }
        }
       
        public void PrintCommandHelpHeader(string executable, string commandName, TextWriter textWriter)
        {
            if (PrintMessages)
            {
                Console.ResetColor();
                textWriter.Write("Usage: ");
                Console.ForegroundColor = ConsoleColor.White;
                textWriter.WriteLine($"{executable} {commandName} [<options>]");
                Console.ResetColor();
                textWriter.WriteLine();
                textWriter.WriteLine("Where [<options>] is any of: ");
                textWriter.WriteLine();
            }
        }

        public void PrintCommandHelpFooter(string executable, string commandName, TextWriter textWriter)
        {
            if (PrintMessages)
            {
                textWriter.WriteLine();
                textWriter.Write("Or use ");
                Console.ForegroundColor = ConsoleColor.White;
                textWriter.Write(executable + " help <command>");
                Console.ResetColor();
                textWriter.WriteLine(" for more details.");
            }
        }

        public void PrintCommandOptions(Options options, TextWriter writer)
        {
            if (PrintMessages)
            {
                foreach (var g in options.OptionSets.Keys.Reverse())
                {
                    writer.WriteLine($"{g}: ");
                    writer.WriteLine();
                    options.OptionSets[g].WriteOptionDescriptions(writer);
                    writer.WriteLine();
                }
            }
        }

        public void Debug(string template, string propertyValue)
        {
            if (PrintMessages)
            {
                logger.Debug(template, propertyValue);
            }
        }

        public void Debug(string template, params object[] propertyValues)
        {
            if (PrintMessages)
            {
                logger.Debug(template, propertyValues);
            }
        }

        public void Information(string template, string propertyValue)
        {
            if (PrintMessages)
            {
                logger.Information(template, propertyValue);
            }
        }

        public void Information(string template, params object[] propertyValues)
        {
            if (PrintMessages)
            {
                logger.Information(template, propertyValues);
            }
        }

        public void Json(object o)
        {
            logger.Information(JsonConvert.SerializeObject(o, Formatting.Indented));
        }

        public void Warning(string s)
        {
            if (PrintMessages)
            {
                logger.Warning(s);
            }
        }

        public void Warning(string template, params object[] propertyValues)
        {
            if (PrintMessages)
            {
                logger.Warning(template, propertyValues);
            }
        }

        public void Error(string template, params object[] propertyValues)
        {
            if (PrintMessages)
            {
                logger.Error(template, propertyValues);
            }
        }
    }
}