using System;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Commands
{
    public class DeleteReleasesCommand : ApiCommand
    {
        public DeleteReleasesCommand(IOctopusSessionFactory client, ILog log) : base(client, log)
        {
        }

        public string ProjectName { get; set; }
        public string MaxVersion { get; set; }
        public string MinVersion { get; set; }
        public bool WhatIf { get; set; }

        public override OptionSet Options
        {
            get
            {
                var options = base.Options;
                options.Add("project=", "Name of the project", v => ProjectName = v);
                options.Add("minversion=", "Minimum (inclusive) version number for the range of versions to delete", v => MinVersion = v);
                options.Add("maxversion=", "Maximum (inclusive) version number for the range of versions to delete", v => MaxVersion = v);
                options.Add("whatif", "[Optional, Flag] if specified, releases won't actually be deleted, but will be listed as if simulating the command", v => WhatIf = true);
                return options;
            }
        }

        public override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
            if (string.IsNullOrWhiteSpace(MinVersion)) throw new CommandException("Please specify a minimum version number using the parameter: --minversion=X.Y.Z");
            if (string.IsNullOrWhiteSpace(MaxVersion)) throw new CommandException("Please specify a maximum version number using the parameter: --maxversion=X.Y.Z");

            var min = SemanticVersion.Parse(MinVersion);
            var max = SemanticVersion.Parse(MaxVersion);

            Log.Debug("Finding project: " + ProjectName);
            var project = Session.GetProject(ProjectName);

            var skip = 0;
            var take = 64;

            Log.Debug("Finding releases for project...");
            while (true)
            {
                var releases = Session.GetReleases(project, skip, take);

                foreach (var release in releases)
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
                            Session.Delete<Release>(release.Link("Self"));
                            Log.InfoFormat("Deleting version {0}", version);
                        }
                    }
                }

                skip += releases.Count;

                if (releases.Count == 0)
                {
                    break;
                }
            }
        }
    }
}