using System;
using Serilog;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octopus.Cli.Importers
{
    public interface IImporterLocator
    {
        IImporterMetadata[] List();
        IImporter Find(string name, IOctopusRepository repository, IOctopusFileSystem fileSystem, ILogger log);
    }
}