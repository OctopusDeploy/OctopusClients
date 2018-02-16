using Octopus.Cli;

namespace Octopus.DotNet.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return new CliProgram().Execute(args);
        }
    }
}
