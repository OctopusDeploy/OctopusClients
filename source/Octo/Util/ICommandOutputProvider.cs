using System.IO;

namespace Octopus.Cli.Util
{
    public interface ICommandOutputProvider
    {
        bool PrintMessages { get; set; }

        void PrintHeader();

        void PrintCommandHelpHeader(string executable, string commandName, TextWriter textWriter);

        void PrintCommandHelpFooter(string executable, string commandName, TextWriter textWriter);

        void Debug(string template, string propertyValue);

        void Debug(string template, params object[] propertyValues);

        void Information(string template, string propertyValue);

        void Information(string template, params object[] propertyValues);

        void Json(object o);
        void Warning(string s);
    }
}