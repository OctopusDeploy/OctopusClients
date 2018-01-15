namespace Octopus.Cli.Exporters
{
    public interface IExporterMetadata
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}