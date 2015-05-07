using System;
using System.Linq;
using Autofac;
using log4net;
using Octopus.Client;
using Octopus.Platform.Util;

namespace OctopusTools.Importers
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
            return
                (from t in typeof (ImporterLocator).Assembly.GetTypes()
                    where typeof (IImporter).IsAssignableFrom(t)
                    let attribute = (IImporterMetadata) t.GetCustomAttributes(typeof (ImporterAttribute), true).FirstOrDefault()
                    where attribute != null
                    select attribute).ToArray();
        }

        public IImporter Find(string name, IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log, bool dryRun = false)
        {
            name = name.Trim().ToLowerInvariant();
            if (dryRun)
                name += "DryRun";
            var found = (from t in typeof (ImporterLocator).Assembly.GetTypes()
                where typeof (IImporter).IsAssignableFrom(t)
                let attribute = (IImporterMetadata) t.GetCustomAttributes(typeof (ImporterAttribute), true).FirstOrDefault()
                where attribute != null
                where attribute.Name == name
                select t).FirstOrDefault();

            return found == null ? null : (IImporter) lifetimeScope.Resolve(found, new TypedParameter(typeof (IOctopusRepository), repository), new TypedParameter(typeof (IOctopusFileSystem), fileSystem), new TypedParameter(typeof (ILog), log));
        }
    }
}