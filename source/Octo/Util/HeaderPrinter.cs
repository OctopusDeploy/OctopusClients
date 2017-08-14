using Serilog;
using Serilog.Core;

namespace Octopus.Cli.Util
{
    public class CommandOutputProvider : ICommandOutputProvider
    {
        public void PrintHeader()
        {
            Log.Information("Octopus Deploy Command Line Tool, version {Version:l}", typeof(Program).GetInformationalVersion());
            Log.Information(string.Empty);
        }

        public void PrintError()
        {
            // TODO
            throw new System.NotImplementedException();
        }
    }
}