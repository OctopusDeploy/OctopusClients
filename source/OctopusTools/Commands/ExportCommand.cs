using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using Octopus.Platform.Util;
using OctopusTools.Exporters;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    [Command("export", Description="Exports an object to a JSON file")]
    public class    ExportCommand : ApiCommand
    {
        readonly IExporterLocator exporterLocator;
        readonly IOctopusFileSystem fileSystem;

        public ExportCommand(IExporterLocator exporterLocator, IOctopusFileSystem fileSystem, IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(repositoryFactory, log)
        {
            this.exporterLocator = exporterLocator;
            this.fileSystem = fileSystem;
        }

        public string Type { get; set; }
        public string FilePath { get; set; }
        public string Project { get; set; }
        public string Name { get; set; }
        public string ReleaseVersion { get; set; }

        protected override void SetOptions(OptionSet options)
        {
            options.Add("type=", "The type to export", v => Type = v);
            options.Add("filePath=", "The full path and name of the export file", v => FilePath = v);
            options.Add("project=", "[Optional] Name of the project", v => Project = v);
            options.Add("name=", "[Optional] Name of the item to export", v => Name = v);
            options.Add("releaseVersion=", "[Optional] The version number, or range of version numbers to export", v => ReleaseVersion = v);
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(Type)) throw new CommandException("Please specify the type to export using the parameter: --type=XYZ");
            if (string.IsNullOrWhiteSpace(FilePath)) throw new CommandException("Please specify the full path and name of the export file using the parameter: --filePath=XYZ");

            Log.Debug("Finding exporter '" + Type + "'");
            var exporter = exporterLocator.Find(Type, Repository, fileSystem, Log);
            if(exporter == null)
                throw new CommandException("Error: Unrecognized exporter '" + Type + "'");
            
            Log.Debug("Beginning the export");
            exporter.Export(string.Format("FilePath={0}", FilePath), string.Format("Project={0}", Project), string.Format("Name={0}", Name), string.Format("ReleaseVersion={0}", ReleaseVersion));
        }
    }
}
