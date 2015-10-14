using System;
using log4net;
using Octopus.Client;
using Octopus.Shared.Util;

namespace OctopusTools.Exporters
{
    public interface IExporterLocator
    {
        IExporterMetadata[] List();
        IExporter Find(string name, IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log);
    }
}