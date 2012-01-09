using System;

namespace OctopusTools.Infrastructure
{
    public class CommandException : Exception
    {
        public CommandException(string message)
            : base(message)
        {
        }
    }
}