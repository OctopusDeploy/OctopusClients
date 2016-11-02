using System;
using System.Linq;
using Autofac;
using Serilog;
using Octopus.Cli.Util;
using Octopus.Client;
using System.Reflection;

namespace Octopus.Cli.Importers
{
    public class ImporterLocator : IImporterLocator
    {
        readonly ILifetimeScope lifetimeScope;

        public ImporterLocator(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public IImporterMetadata[] List()
        {
            var iImporterType = typeof (IImporter).GetTypeInfo();
            return
                (from t in typeof (ImporterLocator).GetTypeInfo().Assembly.GetTypes()
                    where iImporterType.IsAssignableFrom(t)
                    let attribute = (IImporterMetadata) t.GetTypeInfo().GetCustomAttributes(typeof (ImporterAttribute), true).FirstOrDefault()
                    where attribute != null
                    select attribute).ToArray();
        }

        public IImporter Find(string name, IOctopusAsyncRepository repository, IOctopusFileSystem fileSystem, ILogger log)
        {
            var iImporterType = typeof (IImporter).GetTypeInfo();
            name = name.Trim().ToLowerInvariant();
            var found = (from t in typeof (ImporterLocator).GetTypeInfo().Assembly.GetTypes()
                where iImporterType.IsAssignableFrom(t)
                let attribute = (IImporterMetadata) t.GetTypeInfo().GetCustomAttributes(typeof (ImporterAttribute), true).FirstOrDefault()
                where attribute != null
                where attribute.Name == name
                select t).FirstOrDefault();

            return found == null ? null : (IImporter) lifetimeScope.Resolve(found, new TypedParameter(typeof (IOctopusAsyncRepository), repository), new TypedParameter(typeof (IOctopusFileSystem), fileSystem), new TypedParameter(typeof (ILogger), log));
        }
    }
}