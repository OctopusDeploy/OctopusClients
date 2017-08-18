using System;
using System.IO;
using Octo.Model;
using Octopus.Cli.Infrastructure;

namespace Octopus.Cli.Util
{
    public interface ICommandOutputProvider
    {
        bool PrintMessages { get; set; }

        void PrintHeader();

        void PrintError();
        
        void PrintCommandHelpHeader(string executable, string commandName, TextWriter textWriter);

        void PrintCommandHelpFooter(string executable, string commandName, TextWriter textWriter);

        void PrintDebugMessage(string template, string propertyValue);

        void PrintDebugMessage(string template, params object[] propertyValues);

        void PrintInfoMessage(string template, string propertyValue);

        void PrintInfoMessage(string template, params object[] propertyValues);

        void PrintJsonOutput(object o);
    }
}