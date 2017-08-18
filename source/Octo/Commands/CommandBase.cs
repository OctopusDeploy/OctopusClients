using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Serilog;

namespace Octo.Commands
{
    public abstract class CommandBase
    {
        protected readonly ICommandOutputProvider commandOutputProvider;
        protected bool printHelp;

        protected CommandBase(ILogger log, ICommandOutputProvider commandOutputProvider)
        {
            this.commandOutputProvider = commandOutputProvider;
            this.Log = log;

            var options = Options.For("Common options");
            options.Add("help", "[Optional] Print help for a command", x => printHelp = true);
        }

        protected ILogger Log { get; private set; }

        protected Options Options { get; } = new Options();

        public void GetHelp(TextWriter writer, string[] args)
        {
            var typeInfo = this.GetType().GetTypeInfo();

            var executable = Path.GetFileNameWithoutExtension(typeInfo.Assembly.FullLocalPath());
            var commandAttribute = typeInfo.GetCustomAttribute<CommandAttribute>();
            string commandName = string.Empty;
            if (commandAttribute == null)
            {
                commandName = args.FirstOrDefault();
            }
            else
            {
                commandName = commandAttribute.Name;
            }

            commandOutputProvider.PrintCommandHelpHeader(executable, commandName, writer);
            Options.WriteOptionDescriptions(writer);
            commandOutputProvider.PrintCommandHelpFooter(executable, commandName, writer);
        }
    }
}