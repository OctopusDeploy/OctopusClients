using System.Threading.Tasks;
using Octopus.Cli.Exporters;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Serilog;

namespace Octopus.Cli.Commands
{
    [Command("export", Description = "Exports an object to a JSON file")]
    public class ExportCommand : ApiCommand
    {
        readonly IExporterLocator exporterLocator;

        public ExportCommand(IExporterLocator exporterLocator, IOctopusFileSystem fileSystem, IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            this.exporterLocator = exporterLocator;

            var options = Options.For("Export");
            options.Add("type=", "The type to export", v => Type = v);
            options.Add("filePath=", "The full path and name of the export file", v => FilePath = v);
            options.Add("project=", "[Optional] Name of the project", v => Project = v);
            options.Add("name=", "[Optional] Name of the item to export", v => Name = v);
            options.Add("releaseVersion=", "[Optional] The version number, or range of version numbers to export", v => ReleaseVersion = v);
        }

        public string Type { get; set; }
        public string FilePath { get; set; }
        public string Project { get; set; }
        public string Name { get; set; }
        public string ReleaseVersion { get; set; }

        protected override Task Execute()
        {
            if (string.IsNullOrWhiteSpace(Type)) throw new CommandException("Please specify the type to export using the parameter: --type=XYZ");
            if (string.IsNullOrWhiteSpace(FilePath)) throw new CommandException("Please specify the full path and name of the export file using the parameter: --filePath=XYZ");

            commandOutputProvider.Debug("Finding exporter '{Type:l}'", Type);
            var exporter = exporterLocator.Find(Type, Repository, FileSystem, commandOutputProvider);
            if (exporter == null)
                throw new CommandException("Error: Unrecognized exporter '" + Type + "'");

            commandOutputProvider.Debug("Beginning the export");
            return exporter.Export(string.Format("FilePath={0}", FilePath), string.Format("Project={0}", Project), string.Format("Name={0}", Name), string.Format("ReleaseVersion={0}", ReleaseVersion));
        }
    }
}