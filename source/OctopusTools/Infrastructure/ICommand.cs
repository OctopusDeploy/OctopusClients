using System;
using System.IO;

namespace OctopusTools.Infrastructure
{
    public interface ICommand
    {
        void GetHelp(TextWriter writer);
        void Execute(string[] commandLineArguments);
    }
}
