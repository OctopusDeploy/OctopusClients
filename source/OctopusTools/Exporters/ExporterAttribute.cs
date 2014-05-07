using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctopusTools.Exporters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExporterAttribute : Attribute, IExporterMetadata
    {
        public ExporterAttribute(string name, string entityType)
        {
            Name = name;
            EntityType = entityType;
        }
        
        public string Name { get; set; }
        public string Description { get; set; }
        public string EntityType { get; set; }
    }
}
