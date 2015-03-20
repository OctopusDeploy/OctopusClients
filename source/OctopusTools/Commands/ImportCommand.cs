using System;
using log4net;
using Octopus.Platform.Util;
using OctopusTools.Importers;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    [Command("import", Description = "Imports an Octopus object from an export file")]
    public class ImportCommand : ApiCommand
    {
        readonly IOctopusFileSystem fileSystem;
        readonly IImporterLocator importerLocator;

        public ImportCommand(IImporterLocator importerLocator, IOctopusFileSystem fileSystem, IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(repositoryFactory, log)
        {
            this.importerLocator = importerLocator;
            this.fileSystem = fileSystem;

            var options = Options.For("Import");
            options.Add("type=", "The Octopus object type to import", v => Type = v);
            options.Add("filePath=", "The full path and name of the exported file", v => FilePath = v);
            options.Add("project=", "[Optional] The name of the project", v => Project = v);
        }

        public string Type { get; set; }
        public string FilePath { get; set; }
        public string Project { get; set; }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(Type)) throw new CommandException("Please specify the type of object to import using the paramter: --type=XYZ");
            if (string.IsNullOrWhiteSpace(FilePath)) throw new CommandException("Please specify the full path and name of the export file to import using the parameter: --filePath=XYZ");

            Log.Debug("Finding importer '" + Type + "'");
            var importer = importerLocator.Find(Type, Repository, fileSystem, Log);
            if (importer == null)
                throw new CommandException("Error: Unrecognized importer '" + Type + "'");

            Log.Debug("Beginning the import");
            importer.Import(string.Format("FilePath={0}", FilePath), string.Format("Project={0}", Project));
        }
    }
}