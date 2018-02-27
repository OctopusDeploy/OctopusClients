using System.Threading.Tasks;

namespace Octopus.Cli.Exporters
{
    public interface IExporter
    {
        Task Export(params string[] parameters);
    }
}