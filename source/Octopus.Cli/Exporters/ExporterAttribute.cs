using System;

namespace Octopus.Cli.Exporters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExporterAttribute : Attribute, IExporterMetadata
    {
        public ExporterAttribute(string name, string entityType)
        {
            Name = name;
            EntityType = entityType;
        }

        public string EntityType { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}