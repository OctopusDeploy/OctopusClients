using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctopusTools.Exporters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExporterAttribute : Attribute, IExporterMetadata
    {
        public ExporterAttribute(string name)
        {
            Name = name;
        }
        
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
