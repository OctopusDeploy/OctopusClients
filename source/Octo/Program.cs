using Octopus.Cli;

namespace Octo
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return new CliProgram().Execute(args);
        }
    }
}