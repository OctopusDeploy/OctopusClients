using System.Threading.Tasks;

namespace Octopus.Cli.Commands
{
    public interface ISupportFormattedOutput
    {
        Task Request();

        void PrintDefaultOutput();

        void PrintJsonOutput();
    }
}
