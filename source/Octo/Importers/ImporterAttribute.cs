using System;

namespace Octopus.Cli.Importers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImporterAttribute : Attribute, IImporterMetadata
    {
        public ImporterAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}