using System;
using System.Collections.Generic;

namespace OctopusTools.Infrastructure
{
    public class CommandLineArgsProvider : ICommandLineArgsProvider
    {
        public CommandLineArgsProvider(IEnumerable<string> args)
        {
            Args = args;
        }

        public IEnumerable<string> Args { get; set; }
    }
}