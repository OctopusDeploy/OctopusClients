using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctopusTools.Importers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImporterAttribute : Attribute, IImporterMetadata
    {
        public ImporterAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
