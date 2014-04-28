using log4net;
using Newtonsoft.Json;
using Octopus.Client.Model;
using Octopus.Platform.Util;
using OctopusTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctopusTools.Commands
{
    [Command("import-feeds", Description="Imports external feeds")]
    public class ImportFeedsCommand : BaseImportCommand
    {
        public ImportFeedsCommand(IOctopusFileSystem fileSystem, IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(fileSystem, repositoryFactory, log)
        {
        }

        public string FilePath { get; set; }

        protected override void SetOptions(OptionSet options)
        {
            options.Add("filePath=", "Full path and name of the export file to be imported", v => FilePath = v);
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(FilePath)) throw new CommandException("Please specify the full path and name of the export file to be imported using the parameter: --filePath=XYZ");

            var feeds = JsonConvert.DeserializeObject<List<FeedResource>>(GetSerializedObjectFromFile(FilePath));
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
                throw new CommandException(string.Format("Failed to import feeds. Error: {0}", ex.Message));
            }
        }
    }
}
