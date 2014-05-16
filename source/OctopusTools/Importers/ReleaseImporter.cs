using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Platform.Util;
using OctopusTools.Extensions;
using OctopusTools.Infrastructure;

namespace OctopusTools.Importers
{
    [Importer("release", "List", Description = "Imports a projects releases from an export file")]
    public class ReleaseImporter : BaseImporter
    {
        public ReleaseImporter(IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log)
            : base(repository, fileSystem, log)
        {
        }

        //When importing I think we want to just create a new release, using the NuGet package versions, 
        //number and release notes from the exported release
        //we can't import snapshots, so there's no point exporting them
        //instead we'll assume that they have imported the latest project settings and thus the 
        //deployment process + variables will be up to date

        protected override void Import(Dictionary<string, string> paramDictionary)
        {
            if (string.IsNullOrWhiteSpace(paramDictionary["Project"])) throw new CommandException("Please specify the name of the project using the parameter: --project=XYZ");
            var projectName = paramDictionary["Project"];

            var releases = FileSystemImporter.Import<List<ReleaseResource>>(FilePath, typeof (ReleaseImporter).GetAttributeValue((ImporterAttribute ia) => ia.EntityType));
            if (releases == null)
                throw new CommandException("Unable to deserialize the specified export file");

            var project = Repository.Projects.FindByName(projectName);
            if (project == null)
                throw new CommandException("Could not find project named '" + projectName + "'");

            foreach (var release in releases)
            {
                Log.Debug("Importing release '" + release.Version);
                var existingReleases = Repository.Projects.GetReleases(project);

                if (existingReleases == null || existingReleases.Items.All(rls => rls.Version != release.Version))
                {
                    release.ProjectId = project.Id;
                    Log.Debug("Creating new release '" + release.Version + "' for project " + project.Name);
                    Repository.Releases.Create(release);
                }
                else
                {
                    Log.Debug("Release '" + release.Version + "' already exist for project " + project.Name);
                }
            }

            Log.Debug("Successfully imported releases for project " + project.Name);
        }
    }
}