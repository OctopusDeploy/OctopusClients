using System;
using System.Collections.Generic;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Repositories;
using Octopus.Platform.Util;
using OctopusTools.Commands;
using OctopusTools.Extensions;
using OctopusTools.Infrastructure;

namespace OctopusTools.Exporters
{
    [Exporter("libraryvariableset", "LibraryVariableSetWithValues", Description = "Exports a library variable set as JSON to a file")]
    public class LibraryVariableSetExporter : BaseExporter
    {
        public LibraryVariableSetExporter(IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log)
            : base(repository, fileSystem, log)
        {
        }

        protected override void Export(Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters["Name"])) throw new CommandException("Please specify the name of the library variable set to export using the paramater: --name=XYZ");
            var lvsName = parameters["Name"];

            Log.Debug("Finding library variable set: " + lvsName);
            var libraryVariableSet = FindLibraryVariableSetByName(Repository.LibraryVariableSets, lvsName);
            if (libraryVariableSet == null)
                throw new CommandException("Could not find library variable set named: " + lvsName);

            Log.Debug("Finding variable set for library variable set");
            var variables = Repository.VariableSets.Get(libraryVariableSet.VariableSetId);
            if (variables == null)
                throw new CommandException("Could not find variable set for library variable set " + libraryVariableSet.Name);

            var export = new LibraryVariableSetExport
            {
                LibraryVariableSet = libraryVariableSet,
                VariableSet = variables
            };

            var metadata = new ExportMetadata
            {
                ExportedAt = DateTime.Now,
                OctopusVersion = Repository.Client.RootDocument.Version,
                Type = typeof (LibraryVariableSetExporter).GetAttributeValue((ExporterAttribute ea) => ea.Name),
                ContainerType = typeof (LibraryVariableSetExporter).GetAttributeValue((ExporterAttribute ea) => ea.EntityType)
            };
            FileSystemExporter.Export(FilePath, metadata, export);
        }

        private LibraryVariableSetResource FindLibraryVariableSetByName(ILibraryVariableSetRepository lvsRepository, string name)
        {
            name = (name ?? string.Empty).Trim();
            return lvsRepository.FindOne(r => string.Equals((r.Name ?? string.Empty).Trim(), name, StringComparison.OrdinalIgnoreCase));
        }
    }
}