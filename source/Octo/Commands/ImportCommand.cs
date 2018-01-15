using System.Threading.Tasks;
using Octopus.Cli.Importers;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Serilog;

namespace Octopus.Cli.Commands
{
    [Command("import", Description = "Imports an Octopus object from an export file")]
    public class ImportCommand : ApiCommand
    {
        readonly IImporterLocator importerLocator;

        public ImportCommand(IImporterLocator importerLocator, IOctopusFileSystem fileSystem, IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            this.importerLocator = importerLocator;

            var options = Options.For("Import");
            options.Add("type=", "The Octopus object type to import", v => Type = v);
            options.Add("filePath=", "The full path and name of the exported file", v => FilePath = v);
            options.Add("project=", "[Optional] The name of the project", v => Project = v);
            options.Add("dryRun", "[Optional] Perform a dry run of the import", v => DryRun = true);
        }

        public bool DryRun { get; set; }
        public string Type { get; set; }
        public string FilePath { get; set; }
        public string Project { get; set; }

        protected override async Task Execute()
        {
            if (string.IsNullOrWhiteSpace(Type)) throw new CommandException("Please specify the type of object to import using the paramter: --type=XYZ");
            if (string.IsNullOrWhiteSpace(FilePath)) throw new CommandException("Please specify the full path and name of the export file to import using the parameter: --filePath=XYZ");

            commandOutputProvider.Debug("Finding importer '{Type:l}'", Type);
            var importer = importerLocator.Find(Type, Repository, FileSystem, commandOutputProvider);
            if (importer == null)
                throw new CommandException("Error: Unrecognized importer '" + Type + "'");

            commandOutputProvider.Debug("Validating the import");
            var validationResult = await importer.Validate(string.Format("FilePath={0}", FilePath), string.Format("Project={0}", Project)).ConfigureAwait(false);
            if (validationResult && !DryRun)
            {
                commandOutputProvider.Debug("Beginning the import");
                await importer.Import(string.Format("FilePath={0}", FilePath), string.Format("Project={0}", Project)).ConfigureAwait(false);
            }
        }
    }
}