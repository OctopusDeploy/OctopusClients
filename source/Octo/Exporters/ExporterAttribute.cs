using System;

namespace Octopus.Cli.Exporters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExporterAttribute : Attribute, IExporterMetadata
    {
        public ExporterAttribute(string name)
        {
            Name = name;
        }
        

        public string Name { get; set; }
    }
}