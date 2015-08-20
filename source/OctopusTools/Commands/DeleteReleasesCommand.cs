using System;
using System.Collections.Generic;
using log4net;
using NuGet;
using Octopus.Client.Model;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    [Command("delete-releases", Description = "Deletes a range of releases")]
    public class DeleteReleasesCommand : ApiCommand
    {
        public DeleteReleasesCommand(IOctopusRepositoryFactory repositoryFactory, ILog log) : base(repositoryFactory, log)
        {
            var options = Options.For("Deletion");
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("minversion=", "Minimum (inclusive) version number for the range of versions to delete", v => MinVersion = v);
            options.Add("maxversion=", "Maximum (inclusive) version number for the range of versions to delete", v => MaxVersion = v);
            options.Add("whatif", "[Optional, Flag] if specified, releases won't actually be deleted, but will be listed as if simulating the command", v => WhatIf = true);
        }

        public string ProjectName { get; set; }
        public string MaxVersion { get; set; }
        public string MinVersion { get; set; }
        public bool WhatIf { get; set; }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
            if (string.IsNullOrWhiteSpace(MinVersion)) throw new CommandException("Please specify a minimum version number using the parameter: --minversion=X.Y.Z");
            if (string.IsNullOrWhiteSpace(MaxVersion)) throw new CommandException("Please specify a maximum version number using the parameter: --maxversion=X.Y.Z");

            var min = SemanticVersion.Parse(MinVersion);
            var max = SemanticVersion.Parse(MaxVersion);

            Log.Debug("Finding project: " + ProjectName);
            var project = Repository.Projects.FindByName(ProjectName);
            if (project == null)
                throw new CouldNotFindException("a project named", ProjectName);

            Log.Debug("Finding releases for project...");

            var releases = Repository.Projects.GetReleases(project);
            var toDelete = new List<string>();
            while (releases.Items.Count > 0)
            {
                foreach (var release in releases.Items)
                {
                    var version = SemanticVersion.Parse(release.Version);
                    if (min <= version && version <= max)
                    {
                        if (WhatIf)
                        {
                            Log.InfoFormat("[Whatif] Version {0} would have been deleted", version);
                        }
                        else
                        {
                            toDelete.Add(release.Link("Self"));
                            Log.InfoFormat("Deleting version {0}", version);
                        }
                    }
                }

                if (!releases.HasLink("Page.Next"))
                {
                    break;
                }

                releases = Repository.Client.List<ReleaseResource>(releases.Link("Page.Next"));
            }

            if (!WhatIf)
            {
                foreach (var release in toDelete)
                {
                    Repository.Client.Delete(release);
                }
            }
        }
    }
}