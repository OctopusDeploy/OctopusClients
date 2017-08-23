using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Model;
using Octopus.Cli.Util;
using Serilog;

namespace Octopus.Cli.Commands
{
    public abstract class CommandBase
    {
        protected readonly ICommandOutputProvider commandOutputProvider;
        protected bool printHelp;
        protected readonly ISupportFormattedOutput formattedOutputInstance;

        protected CommandBase(ILogger log, ICommandOutputProvider commandOutputProvider)
        {
            this.commandOutputProvider = commandOutputProvider;
            this.Log = log;

            var options = Options.For("Common options");
            options.Add("help", "[Optional] Print help for a command", x => printHelp = true);
            formattedOutputInstance = this as ISupportFormattedOutput;
            if (formattedOutputInstance != null)
            {
                options.Add("outputFormat=", "[Optional] Output format, valid options are json or xml",
                    SetOutputFormat);
            }
            else
            {
                commandOutputProvider.PrintMessages = true;
            }
        }

        protected ILogger Log { get; private set; }

        protected Options Options { get; } = new Options();

        public OutputFormat OutputFormat { get; set; }

        public virtual void GetHelp(TextWriter writer, string[] args)
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

            commandOutputProvider.PrintMessages = OutputFormat == OutputFormat.Default;
            if (OutputFormat == OutputFormat.Json)
            {
                commandOutputProvider.Json(new
                {
                    Command = commandName,
                    Options = Options.OptionSets.OrderByDescending(x => x.Key).Select(g => new
                    {
                        @Group = g.Key,
                        Parameters = g.Value.Select(p => new
                        {
                            Name = p.Names.First(),
                            Usage = string.Format("{0}{1}{2}",p.Prototype.Length == 1 ? "-" : "--", p.Prototype, p.Prototype.EndsWith("=") ? "VALUE" : string.Empty),
                            Value = p.OptionValueType.ToString(),
                            p.Description
                        })
                    })
                });
            }
            else
            {
                
                commandOutputProvider.PrintCommandHelpHeader(executable, commandName, writer);
                commandOutputProvider.PrintCommandOptions(Options, writer);
                commandOutputProvider.PrintCommandHelpFooter(executable, commandName, writer);
            }
        }

        private void SetOutputFormat(string s)
        {
            OutputFormat outputFormat;
            OutputFormat = Enum.TryParse(s, true, out outputFormat) ? outputFormat : OutputFormat.Default;
            
        }
    }
}