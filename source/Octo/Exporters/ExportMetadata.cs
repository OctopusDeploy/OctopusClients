using System;

namespace Octopus.Cli.Exporters
{
    public class ExportMetadata
    {
        public DateTime ExportedAt { get; set; }
        public string OctopusVersion { get; set; }
        public string Type { get; set; }
        public string ContainerType { get; set; }
    }
}