using System;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands
{
    [Command("version", "v", "ver", Description = "Output Octo command line tool version.")]
    public class VersionCommand : CommandBase, ICommand
    {
        public VersionCommand(ICommandOutputProvider commandOutputProvider) : base(commandOutputProvider)
        {
        }

        public Task Execute(string[] commandLineArgs)
        {
            return Task.Run(() => 
            {
                Console.WriteLine($"{typeof(CliProgram).GetInformationalVersion()}");
            });
        }

    }
}
