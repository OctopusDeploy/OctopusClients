using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Serilog;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octopus.Cli.Exporters
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
            var iExporterType = typeof (IExporter).GetTypeInfo();
            return
                (from t in typeof (ExporterLocator).GetTypeInfo().Assembly.GetTypes()
                    where iExporterType.IsAssignableFrom(t)
                    let attribute = (IExporterMetadata) t.GetTypeInfo().GetCustomAttributes(typeof (ExporterAttribute), true).FirstOrDefault()
                    where attribute != null
                    select attribute).ToArray();
        }

        public IExporter Find(string name, IOctopusRepository repository, IOctopusFileSystem fileSystem, ILogger log)
        {
            var iExporterType = typeof (IExporter).GetTypeInfo();
            name = name.Trim().ToLowerInvariant();
            var found = (from t in typeof (ExporterLocator).GetTypeInfo().Assembly.GetTypes()
                where iExporterType.IsAssignableFrom(t)
                let attribute = (IExporterMetadata) t.GetTypeInfo().GetCustomAttributes(typeof (ExporterAttribute), true).FirstOrDefault()
                where attribute != null
                where attribute.Name == name
                select t).FirstOrDefault();

            return found == null ? null : (IExporter) lifetimeScope.Resolve(found, new TypedParameter(typeof (IOctopusRepository), repository), new TypedParameter(typeof (IOctopusFileSystem), fileSystem), new TypedParameter(typeof (ILogger), log));
        }
    }
}