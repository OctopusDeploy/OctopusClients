namespace Octopus.Cli.Importers
{
    public interface IImporterMetadata
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}