using System;
using System.Linq;
using Autofac;
using log4net;
using Octopus.Client;
using Octopus.Platform.Util;

namespace OctopusTools.Exporters
{
    public class ExporterLocator : IExporterLocator
    {
        readonly ILifetimeScope lifetimeScope;

        public ExporterLocator(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public IExporterMetadata[] List()
        {
            return
                (from t in typeof (ExporterLocator).Assembly.GetTypes()
                    where typeof (IExporter).IsAssignableFrom(t)
                    let attribute = (IExporterMetadata) t.GetCustomAttributes(typeof (ExporterAttribute), true).FirstOrDefault()
                    where attribute != null
                    select attribute).ToArray();
        }

        public IExporter Find(string name, IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log)
        {
            name = name.Trim().ToLowerInvariant();
            var found = (from t in typeof (ExporterLocator).Assembly.GetTypes()
                where typeof (IExporter).IsAssignableFrom(t)
                let attribute = (IExporterMetadata) t.GetCustomAttributes(typeof (ExporterAttribute), true).FirstOrDefault()
                where attribute != null
                where attribute.Name == name
                select t).FirstOrDefault();

            return found == null ? null : (IExporter) lifetimeScope.Resolve(found, new TypedParameter(typeof (IOctopusRepository), repository), new TypedParameter(typeof (IOctopusFileSystem), fileSystem), new TypedParameter(typeof (ILog), log));
        }
    }
}