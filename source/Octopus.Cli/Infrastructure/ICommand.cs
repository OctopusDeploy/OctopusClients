using System;
using System.IO;

namespace Octopus.Cli.Infrastructure
{
    public interface ICommand
    {
        void GetHelp(TextWriter writer);
        void Execute(string[] commandLineArguments);
    }
}