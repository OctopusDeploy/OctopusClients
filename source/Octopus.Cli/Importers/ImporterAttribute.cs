using System;

namespace Octopus.Cli.Importers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImporterAttribute : Attribute, IImporterMetadata
    {
        public ImporterAttribute(string name, string entityType)
        {
            Name = name;
            EntityType = entityType;
        }

        public string EntityType { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}