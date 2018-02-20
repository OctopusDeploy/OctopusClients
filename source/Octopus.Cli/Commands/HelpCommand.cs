using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Model;
using Octopus.Cli.Util;
using Serilog;

namespace Octopus.Cli.Commands
{
    [Command("help", "?", "h", Description = "Prints this help text")]
    public class HelpCommand : CommandBase, ICommand
    {
        readonly ICommandLocator commands;
        string executable;

        public HelpCommand(ICommandLocator commands, ICommandOutputProvider commandOutputProvider) : base(commandOutputProvider)
        {
            this.commands = commands;
        }

        public Task Execute(string[] commandLineArguments)
        {
            return Task.Run(() =>
            {
                Options.Parse(commandLineArguments);

                commandOutputProvider.PrintMessages = OutputFormat == OutputFormat.Default;

                executable = "octo";

                var commandName = commandLineArguments.FirstOrDefault();

                if (string.IsNullOrEmpty(commandName))
                {
                    PrintGeneralHelp();
                }
                else
                {
                    var command = commands.Find(commandName);

                    if (command == null)
                    {
                        if (!commandName.StartsWith("--"))
                        {
                            // wasn't a parameter!
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Command '{0}' is not supported", commandName);
                        }
                        Console.ResetColor();
                        PrintGeneralHelp();
                    }
                    else
                    {
                        PrintCommandHelp(command, commandLineArguments);
                    }
                }
            });
        }

        void PrintCommandHelp(ICommand command, string[] args)
        {
            command.GetHelp(Console.Out, args);
        }

        void PrintGeneralHelp()
        {
            if (HelpOutputFormat == OutputFormat.Json)
            {
                PrintJsonOutput();
            }
            else
            {
                PrintDefaultOutput();
            }
        }

        public Task Request()
        {
            return Task.WhenAny();
        }

        public void PrintDefaultOutput()
        {
            Console.ResetColor();
            Console.Write("Usage: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(executable + " <command> [<options>]");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Where <command> is one of: ");
            Console.WriteLine();

            foreach (var possible in commands.List().OrderBy(x => x.Name))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("  " + possible.Name.PadRight(15, ' '));
                Console.ResetColor();
                Console.WriteLine("   " + possible.Description);
            }

            Console.WriteLine();
            Console.Write("Or use ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(executable + " help <command>");
            Console.ResetColor();
            Console.WriteLine(" for more details.");
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(commands.List().Select(x => new
            {
                x.Name,
                x.Description,
                x.Aliases
            }));
        }
    }
}