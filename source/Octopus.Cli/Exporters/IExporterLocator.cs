using System;
using Serilog;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octopus.Cli.Exporters
{
    public interface IExporterLocator
    {
        IExporterMetadata[] List();
        IExporter Find(string name, IOctopusRepository repository, IOctopusFileSystem fileSystem, ILogger log);
    }
}