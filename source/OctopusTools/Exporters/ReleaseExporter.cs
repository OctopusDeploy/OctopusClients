using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using NuGet;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Platform.Util;
using OctopusTools.Extensions;
using OctopusTools.Infrastructure;

namespace OctopusTools.Exporters
{
    [Exporter("release", "List", Description = "Exports either a single release, or multiple releases")]
    public class ReleaseExporter : BaseExporter
    {
        public ReleaseExporter(IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log) :
            base(repository, fileSystem, log)
        {
        }

        protected override void Export(Dictionary<string, string> paramDictionary)
        {
            if (string.IsNullOrWhiteSpace(paramDictionary["Project"])) throw new CommandException("Please specify the project name using the parameter: --project=XYZ");
            if (string.IsNullOrWhiteSpace(paramDictionary["ReleaseVersion"])) throw new CommandException("Please specify the release, or range of releases using the parameter: --releaseVersion=1.0.0 for a single release, or --releaseVersion=1.0.0->1.0.3 for a range of releases");
            var projectName = paramDictionary["Project"];
            var releaseVersion = paramDictionary["ReleaseVersion"];

            Log.Debug("Finding project: " + projectName);
            var project = Repository.Projects.FindByName(projectName);
            if (project == null)
                throw new CommandException("Could not find a project named: " + projectName);

            Log.Debug("Finding releases for project...");
            var releases = Repository.Projects.GetReleases(project);

            var releasesToExport = new List<ReleaseResource>();
            SemanticVersion minVersionToExport;
            SemanticVersion maxVersionToExport;

            if (releaseVersion.IndexOf("->", StringComparison.Ordinal) > 0)
            {
                var releaseVersions = releaseVersion.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
                if (releaseVersions.Count() > 2)
                    throw new CommandException("Incorrect format for exporting multiple releases, please specify the release versions as 1.0.0->1.0.3");
                minVersionToExport = SemanticVersion.Parse(releaseVersions[0]);
                maxVersionToExport = SemanticVersion.Parse(releaseVersions[1]);
            }
            else if (releaseVersion.IndexOf("-", StringComparison.Ordinal) > 0)
            {
                var releaseVersions = releaseVersion.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
                if (releaseVersions.Count() > 2)
                    throw new CommandException("Incorrect format for exporting multiple releases, please specify the release versions as 1.0.0-1.0.3");

                minVersionToExport = SemanticVersion.Parse(releaseVersions[0]);
                if (!SemanticVersion.TryParse(releaseVersions[1], out maxVersionToExport))
                {
                    minVersionToExport = SemanticVersion.Parse(releaseVersion);
                    maxVersionToExport = minVersionToExport;
                }
            }
            else
            {
                minVersionToExport = SemanticVersion.Parse(releaseVersion);
                maxVersionToExport = minVersionToExport;
            }

            while (releases.Items.Count > 0)
            {
                foreach (var release in releases.Items)
                {
                    var version = SemanticVersion.Parse(release.Version);
                    if (minVersionToExport <= version && version <= maxVersionToExport)
                    {
                        Log.Debug("Found release " + version);
                        releasesToExport.Add(release);

                        if (minVersionToExport == maxVersionToExport)
                        {
                            break;
                        }
                    }
                }

                if (((minVersionToExport == maxVersionToExport) && releasesToExport.Count == 1) || !releases.HasLink("Page.Next"))
                {
                    break;
                }

                releases = Repository.Client.List<ReleaseResource>(releases.Link("Next"));
            }

            var metadata = new ExportMetadata
            {
                ExportedAt = DateTime.Now,
                OctopusVersion = Repository.Client.RootDocument.Version,
                Type = typeof (ReleaseExporter).GetAttributeValue((ExporterAttribute ea) => ea.Name),
                ContainerType = typeof (ReleaseExporter).GetAttributeValue((ExporterAttribute ea) => ea.EntityType)
            };
            FileSystemExporter.Export(FilePath, metadata, releasesToExport);
        }
    }
}