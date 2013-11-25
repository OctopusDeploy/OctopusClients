using System;
using System.IO;
using System.Linq;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    [Command("help", "?", "h", Description = "Prints this help text")]
    public class HelpCommand : ICommand
    {
        readonly ICommandLocator commands;

        public HelpCommand(ICommandLocator commands)
        {
            this.commands = commands;
        }

        public void GetHelp(TextWriter writer)
        {
        }

        public void Execute(string[] commandLineArguments)
        {
            var executable = Path.GetFileNameWithoutExtension(typeof(HelpCommand).Assembly.FullLocalPath());

            var commandName = commandLineArguments.FirstOrDefault();

            if (string.IsNullOrEmpty(commandName))
            {
                PrintGeneralHelp(executable);
            }
            else
            {
                var command = commands.Find(commandName);

                if (command == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Command '{0}' is not supported", commandName);
                    Console.ResetColor();
                    PrintGeneralHelp(executable);
                }
                else
                {
                    PrintCommandHelp(executable, command, commandName);
                }
            }
        }

        void PrintCommandHelp(string executable, ICommand command, string commandName)
        {
            Console.ResetColor();
            Console.Write("Usage: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(executable + " " + commandName + " [<options>]");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Where [<options>] is any of: ");
            Console.WriteLine();

            command.GetHelp(Console.Out);

            Console.WriteLine();
            Console.Write("Or use ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(executable + " help <command>");
            Console.ResetColor();
            Console.WriteLine(" for more details.");
        }

        void PrintGeneralHelp(string executable)
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
    }
}