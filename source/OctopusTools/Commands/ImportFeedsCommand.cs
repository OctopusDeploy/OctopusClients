using log4net;
using Octopus.Client.Model;
using Octopus.Platform.Util;
using OctopusTools.Importers;
using System;
using System.Collections.Generic;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    public class ImportFeedsCommand : ApiCommand
    {
        readonly FileSystemImporter importer;
        public ImportFeedsCommand(IOctopusFileSystem fileSystem, IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(repositoryFactory, log)
        {
            importer = new FileSystemImporter(fileSystem, log);
        }

        public string FilePath { get; set; }

        protected override void SetOptions(OptionSet options)
        {
            options.Add("filePath=", "Full path and name of the export file to be imported", v => FilePath = v);
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(FilePath)) throw new CommandException("Please specify the full path and name of the export file to be imported using the parameter: --filePath=XYZ");

            var feeds = importer.Import<List<FeedResource>>(FilePath);
            if(feeds.Count == 0)
            {
                Log.Debug("Found no feeds for import.");
                return;
            }

            try
            {
                foreach (var feed in feeds)
                {
                    var existingFeed = Repository.Feeds.FindByName(feed.Name);
                    if (existingFeed != null)
                    {
                        existingFeed.FeedUri = feed.FeedUri;
                        existingFeed.Username = feed.Username;

                        Repository.Feeds.Modify(existingFeed);
                        Log.DebugFormat("Updated existing feed. ID: {0}", existingFeed.Id);
                    } 
                    else
                    {
                        var newFeed = Repository.Feeds.Create(feed);
                        Log.DebugFormat("Created new feed. ID: {0}", newFeed.Id);
                    }
                }
                Log.DebugFormat("Successfully imported {0} {1}", feeds.Count, feeds.Count == 1 ? "feed" : "feeds");
            }
            catch(Exception ex)
            {
                Log.DebugFormat("Failed to import feeds...");
                throw;
            }
        }
    }
}
