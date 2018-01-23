using System.IO;
using System.Threading.Tasks;

namespace Octopus.Cli.Infrastructure
{
    public interface ICommand
    {
        void GetHelp(TextWriter writer, string[] args);
        Task Execute(string[] commandLineArguments);
    }
}