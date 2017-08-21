using System;
using System.IO;
using Newtonsoft.Json;
using Octo.Model;
using Octopus.Cli.Infrastructure;
using Serilog;
using Serilog.Core;

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
            Console.ResetColor();
            textWriter.Write("Usage: ");
            Console.ForegroundColor = ConsoleColor.White;
            textWriter.WriteLine($"{executable} {commandName} [<options>]");
            Console.ResetColor();
            textWriter.WriteLine();
            textWriter.WriteLine("Where [<options>] is any of: ");
            textWriter.WriteLine();
        }

        public void PrintCommandHelpFooter(string executable, string commandName, TextWriter textWriter)
        {
            textWriter.WriteLine();
            textWriter.Write("Or use ");
            Console.ForegroundColor = ConsoleColor.White;
            textWriter.Write(executable + " help <command>");
            Console.ResetColor();
            textWriter.WriteLine(" for more details.");
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
    }
}