using log4net;
using Newtonsoft.Json;
using Octopus.Platform.Util;
using OctopusTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctopusTools.Commands
{
    [Command("export-feeds", Description = "Exports all External Feeds from the Library")]
    public class ExportFeedsCommand : ApiCommand
    {
        readonly IOctopusFileSystem fileSystem;
        public ExportFeedsCommand(IOctopusFileSystem fileSystem, IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(repositoryFactory, log)
        {
            this.fileSystem = fileSystem;
        }

        public string FilePath { get; set; }
        protected override void SetOptions(OptionSet options)
        {
            options.Add("filePath=", "Full path and name of export file", v => FilePath = v);
        }
        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(FilePath)) throw new CommandException("Please specify the full path and name for the export file using the parameter: --filePath=XYZ");
 
            Log.Debug("Retrieving all external Feeds");

            var feeds = Repository.Feeds.FindMany(f => !f.FeedUri.StartsWith("octopus://"));
            Log.DebugFormat("Found {0} external feeds", feeds.Count);

            var export = JsonConvert.SerializeObject(feeds);

            try
            {
                using (var fileStream = fileSystem.OpenFile(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    var bytes = Encoding.UTF8.GetBytes(export);
                    fileStream.Write(bytes, 0, bytes.Length);
                }
                Log.DebugFormat("Export file {0} successfully created.", FilePath);
            }
            catch (Exception ex)
            {
                Log.DebugFormat("Failed to write file {0} to file system. {1}", FilePath, ex.Message);
            }
        }
    }
}
