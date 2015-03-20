using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using NuGet;
using Octopus.Platform.Util;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    [Command("pack", Description = "Creates a NUPKG from files on disk, without a .NUSPEC or .CSPROJ")]
    public class PackCommand : ICommand
    {
        readonly IList<string> authors = new List<string>();
        readonly IOctopusFileSystem fileSystem;
        readonly IList<string> includes = new List<string>();
        readonly ILog log;
        readonly OptionSet options;
        string basePath;
        string description;
        string id;
        string outFolder;
        bool overwrite;
        string releaseNotes, releaseNotesFile;
        string title;
        SemanticVersion version;

        public PackCommand(ILog log, IOctopusFileSystem fileSystem)
        {
            this.log = log;
            this.fileSystem = fileSystem;

            options = new OptionSet
            {
                {"id=", "The ID of the package; e.g. MyCompany.MyApp", v => id = v},
                {"overwrite", "[Optional] Allow an existing package file of the same ID/version to be overwritten", v => overwrite = true},
                {"include=", "[Optional, Multiple] Add a file pattern to include, relative to the base path e.g. /bin/*.dll - if none are specified, defaults to **", v => includes.Add(v)},
                {"basePath=", "[Optional] The root folder containing files and folders to pack; defaults to '.'", v => basePath = v},
                {"outFolder=", "[Optional] The folder into which the generated NUPKG file will be written; defaults to '.'", v => outFolder = v},
                {"version=", "[Optional] The version of the package; must be a valid SemVer; defaults to a timestamp-based version", v => version = string.IsNullOrWhiteSpace(v) ? null : new SemanticVersion(v) },
                {"author=", "[Optional, Multiple] Add an author to the package metadata; defaults to the current user", v => authors.Add(v)},
                {"title=", "[Optional] The title of the package", v => title = v},
                {"description=", "[Optional] A description of the package; defaults to a generic description", v => description = v},
                {"releaseNotes=", "[Optional] Release notes for this version of the package", v => releaseNotes = v},
                {"releaseNotesFile=", "[Optional] A file containing release notes for this version of the package", v => releaseNotesFile = v}
            };
        }

        public void GetHelp(TextWriter writer)
        {
            options.WriteOptionDescriptions(writer);
        }

        public void Execute(string[] commandLineArguments)
        {
            options.Parse(commandLineArguments);

            if (string.IsNullOrWhiteSpace(id))
                throw new CommandException("An ID is required");

            if (includes.All(string.IsNullOrWhiteSpace))
                includes.Add("**");

            if (string.IsNullOrWhiteSpace(basePath))
                basePath = Path.GetFullPath(Environment.CurrentDirectory);

            if (string.IsNullOrWhiteSpace(outFolder))
                outFolder = Path.GetFullPath(Environment.CurrentDirectory);

            if (version == null)
            {
                var now = DateTime.Now;
                version = new SemanticVersion(now.Year, now.Month, now.Day, now.Hour*10000 + now.Minute*100 + now.Second);
            }
            else
            {
                // Make sure SpecialVersion has 20 characters maximum (Limit imposed by NuGet)
                // https://nuget.codeplex.com/workitem/3426

                const int nugetSpecialVersionMaxLength = 20;
                if (!string.IsNullOrWhiteSpace(version.SpecialVersion) && version.SpecialVersion.Length > nugetSpecialVersionMaxLength)
                {
                    log.WarnFormat("SpecialVersion '{0}' will be truncated to {1} characters (NuGet limit)",
                        version.SpecialVersion, nugetSpecialVersionMaxLength);

                    var specialVersion = version.SpecialVersion;
                    specialVersion = specialVersion.Substring(0, Math.Min(nugetSpecialVersionMaxLength, specialVersion.Length));

                    version = new SemanticVersion(version.Version, specialVersion);
                }
            }

            if (authors.All(string.IsNullOrWhiteSpace))
                authors.Add(Environment.UserName + "@" + Environment.UserDomainName);

            if (string.IsNullOrWhiteSpace(description))
                description = "A deployment package created from files on disk.";

            string allReleaseNotes = null;
            if (!string.IsNullOrWhiteSpace(releaseNotesFile))
            {
                if (!File.Exists(releaseNotesFile))
                    log.WarnFormat("The release notes file '{0}' could not be found", releaseNotesFile);
                else
                    allReleaseNotes = fileSystem.ReadFile(releaseNotesFile);
            }

            if (!string.IsNullOrWhiteSpace(releaseNotes))
            {
                if (allReleaseNotes != null)
                    allReleaseNotes += Environment.NewLine + releaseNotes;
                else
                    allReleaseNotes = releaseNotes;
            }

            var metadata = new ManifestMetadata
            {
                Id = id,
                Authors = string.Join(", ", authors),
                Description = description,
                Version = version.ToString(),
            };

            if (!string.IsNullOrWhiteSpace(allReleaseNotes))
                metadata.ReleaseNotes = allReleaseNotes;

            if (!string.IsNullOrWhiteSpace(title))
                metadata.Title = title;

            log.InfoFormat("Packing {0} version {1}...", id, version);

            var package = new PackageBuilder();

            package.PopulateFiles(basePath, includes.Select(i => new ManifestFile {Source = i}));
            package.Populate(metadata);

            var filename = metadata.Id + "." + metadata.Version + ".nupkg";
            var output = Path.Combine(outFolder, filename);

            if (fileSystem.FileExists(output) && !overwrite)
                throw new CommandException("The package file already exists and --overwrite was not specified");

            log.InfoFormat("Saving {0} to {1}...", filename, outFolder);

            fileSystem.EnsureDirectoryExists(outFolder);

            using (var outStream = fileSystem.OpenFile(output, FileMode.Create))
                package.Save(outStream);

            log.InfoFormat("Done.");
        }
    }
}