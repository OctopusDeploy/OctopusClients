using System;

namespace Octopus.Cli.Infrastructure
{
    public class CommandException : Exception
    {
        public CommandException(string message)
            : base(message)
        {
        }
    }
}