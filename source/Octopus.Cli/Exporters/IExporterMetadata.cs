using System;

namespace OctopusTools.Exporters
{
    public interface IExporterMetadata
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}