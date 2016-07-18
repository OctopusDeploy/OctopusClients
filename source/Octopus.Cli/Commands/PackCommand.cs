using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using NuGet;
using NuGet.Versioning;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands
{
    [Command("pack", Description = "Creates a package (.nupkg or .zip) from files on disk, without needing a .nuspec or .csproj")]
    public class PackCommand : ICommand
    {
        readonly IList<string> authors = new List<string>();
        readonly IOctopusFileSystem fileSystem;
        readonly IList<string> includes = new List<string>();
        readonly ILog log;
        string basePath;
        string description;
        string id;
        string outFolder;
        bool overwrite;
        string releaseNotes, releaseNotesFile;
        string title;
        NuGetVersion version;
        readonly Options optionGroups = new Options();
        IPackageBuilder packageBuilder;

        public PackCommand(ILog log, IOctopusFileSystem fileSystem)
        {
            this.log = log;
            this.fileSystem = fileSystem;

            var common = optionGroups.For("Advanced options");
            common.Add("include=", "[Optional, Multiple] Add a file pattern to include, relative to the base path e.g. /bin/*.dll - if none are specified, defaults to **", v => includes.Add(v));
            common.Add("overwrite", "[Optional] Allow an existing package file of the same ID/version to be overwritten", v => overwrite = true);

            var nuget = optionGroups.For("NuGet packages");
            nuget.Add("author=", "[Optional, Multiple] Add an author to the package metadata; defaults to the current user", v => authors.Add(v));
            nuget.Add("title=", "[Optional] The title of the package", v => title = v);
            nuget.Add("description=", "[Optional] A description of the package; defaults to a generic description", v => description = v);
            nuget.Add("releaseNotes=", "[Optional] Release notes for this version of the package", v => releaseNotes = v);
            nuget.Add("releaseNotesFile=", "[Optional] A file containing release notes for this version of the package", v => releaseNotesFile = v);
            
            var basic = optionGroups.For("Basic options");
            basic.Add("id=", "The ID of the package; e.g. MyCompany.MyApp", v => id = v);
            basic.Add("format=", "Package format. Options are: NuPkg, Zip. Defaults to NuPkg, though we recommend Zip going forward.", fmt => packageBuilder = SelectFormat(fmt));
            basic.Add("version=", "[Optional] The version of the package; must be a valid SemVer; defaults to a timestamp-based version", v => version = string.IsNullOrWhiteSpace(v) ? null : new NuGetVersion(v));
            basic.Add("outFolder=", "[Optional] The folder into which the generated NUPKG file will be written; defaults to '.'", v => outFolder = v);
            basic.Add("basePath=", "[Optional] The root folder containing files and folders to pack; defaults to '.'", v => basePath = v);

            packageBuilder = SelectFormat("nupkg");
        }

        public void GetHelp(TextWriter writer)
        {
            optionGroups.WriteOptionDescriptions(writer);
        }

        public void Execute(string[] commandLineArguments)
        {
            optionGroups.Parse(commandLineArguments);

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
                version = new NuGetVersion(now.Year, now.Month, now.Day, now.Hour*10000 + now.Minute*100 + now.Second);
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

            packageBuilder.BuildPackage(basePath, includes, metadata, outFolder, overwrite);

            log.InfoFormat("Done.");
        }

        IPackageBuilder SelectFormat(string fmt)
        {
            switch (fmt.ToLowerInvariant())
            {
                case "zip":
                    return new ZipPackageBuilder(fileSystem, log);
                case "nupkg":
                case "nuget":
                    return new NuGetPackageBuilder(fileSystem, log);
                default:
                    throw new CommandException("Unknown package format: " + fmt);
            }
        }
    }
}