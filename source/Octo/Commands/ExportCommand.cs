using System;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Exporters;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octopus.Cli.Commands
{
    [Command("export", Description = "Exports an object to a JSON file")]
    public class ExportCommand : ApiCommand
    {
        readonly IExporterLocator exporterLocator;

        public ExportCommand(IExporterLocator exporterLocator, IOctopusFileSystem fileSystem, IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
            this.exporterLocator = exporterLocator;

            var options = Options.For("Export");
            options.Add("type=", "The type to export, either Project, Release or Variables", v => Type = v);
            options.Add("filePath=", "The full path and name of the export file", v => FilePath = v);
            options.Add("name=", "Name of the item to export (only for --type=Project)", v => Name = v);
            options.Add("project=", "Name of the project (only for --type=Release or --type=Variables)", v => Project = v);
            options.Add("releaseVersion=", "The version number, or range of version numbers to export (only for --type=Release)", v => ReleaseVersion = v);
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

            Log.Debug("Finding exporter '{Type:l}'", Type);
            var exporter = exporterLocator.Find(Type, Repository, FileSystem, Log);
            if (exporter == null)
                throw new CommandException("Error: Unrecognized exporter '" + Type + "'");

            Log.Debug("Beginning the export");
            return exporter.Export($"FilePath={FilePath}", $"Project={Project}", $"Name={Name}", $"ReleaseVersion={ReleaseVersion}");
        }
    }
}