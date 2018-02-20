using Octopus.Cli.Util;
using Octopus.Client;
using Serilog;

namespace Octopus.Cli.Exporters
{
    public interface IExporterLocator
    {
        IExporterMetadata[] List();
        IExporter Find(string name, IOctopusAsyncRepository repository, IOctopusFileSystem fileSystem, ICommandOutputProvider commandOutputProvider);
    }
}