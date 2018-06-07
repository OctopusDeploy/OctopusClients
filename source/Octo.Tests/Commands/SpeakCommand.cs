using System.Threading.Tasks;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;

namespace Octo.Tests.Commands
{
    public class SpeakCommand : CommandBase, ICommand
    {
        //public OptionSet Options
        //{
        //    get
        //    {
        //        var options = new OptionSet();
                
        //        return options;
        //    }
        //}

        //public void GetHelp(TextWriter writer, string[] args)
        //{
        //    ICommandOutputProvider commandOutputProvider = new CommandOutputProvider();
        //    commandOutputProvider.PrintCommandHelpHeader("executable", "speak", writer);
        //    writer.WriteLine("message=");
        //    Options.WriteOptionDescriptions(writer);
        //    commandOutputProvider.PrintCommandHelpFooter("executable", "speak", writer);
        //}

        public Task Execute(string[] commandLineArguments)
        {
            return Task.Run(() => Assert.Fail("This should not be called"));
        }

        public SpeakCommand(ICommandOutputProvider commandOutputProvider) : base(commandOutputProvider)
        {
            var options = Options.For("default");
            options.Add("message=", m => { });
        }
    }
}