using log4net;
using Octopus.Cli.Importers;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands
{
    [Command("import", Description = "Imports an Octopus object from an export file")]
    public class ImportCommand : ApiCommand
    {
        readonly IImporterLocator importerLocator;

        public ImportCommand(IImporterLocator importerLocator, IOctopusFileSystem fileSystem, IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(repositoryFactory, log, fileSystem)
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

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(Type)) throw new CommandException("Please specify the type of object to import using the paramter: --type=XYZ");
            if (string.IsNullOrWhiteSpace(FilePath)) throw new CommandException("Please specify the full path and name of the export file to import using the parameter: --filePath=XYZ");

            Log.Debug("Finding importer '" + Type + "'");
            var importer = importerLocator.Find(Type, Repository, FileSystem, Log);
            if (importer == null)
                throw new CommandException("Error: Unrecognized importer '" + Type + "'");

            Log.Debug("Validating the import");
            var validationResult = importer.Validate(string.Format("FilePath={0}", FilePath), string.Format("Project={0}", Project));
            if (validationResult && !DryRun)
            {
                Log.Debug("Beginning the import");
                importer.Import(string.Format("FilePath={0}", FilePath), string.Format("Project={0}", Project));
            }
        }
    }
}