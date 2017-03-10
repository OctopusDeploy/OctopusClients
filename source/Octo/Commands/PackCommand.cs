using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using NuGet.Packaging;
using NuGet.Versioning;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using SemanticVersion = Octopus.Client.Model.SemanticVersion;

namespace Octopus.Cli.Commands
{
    [Command("pack", Description = "Creates a package (.nupkg or .zip) from files on disk, without needing a .nuspec or .csproj")]
    public class PackCommand : ICommand
    {
        readonly IList<string> authors = new List<string>();
        readonly IOctopusFileSystem fileSystem;
        readonly IList<string> includes = new List<string>();
        readonly ILogger log;
        string basePath;
        string description;
        string id;
        string outFolder;
        bool overwrite;
        bool verbose;
        string releaseNotes, releaseNotesFile;
        string title;
        SemanticVersion version;
        readonly Options optionGroups = new Options();
        IPackageBuilder packageBuilder;

        public PackCommand(ILogger log, IOctopusFileSystem fileSystem)
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
            basic.Add("format=", "Package format. Options are: NuPkg, Zip. Defaults to NuPkg, though we recommend Zip going forward", fmt => packageBuilder = SelectFormat(fmt));
            basic.Add("version=", "[Optional] The version of the package; must be a valid SemVer; defaults to a timestamp-based version", v => version = string.IsNullOrWhiteSpace(v) ? null : new SemanticVersion(v));
            basic.Add("outFolder=", "[Optional] The folder into which the generated NUPKG file will be written; defaults to '.'", v => { v.CheckForIllegalPathCharacters(nameof(outFolder)); outFolder = v;});
            basic.Add("basePath=", "[Optional] The root folder containing files and folders to pack; defaults to '.'", v => { v.CheckForIllegalPathCharacters(nameof(basePath)); basePath = v;});
            basic.Add("verbose", "[Optional] verbose output", v => verbose = true);

            packageBuilder = SelectFormat("nupkg");
        }

        public void GetHelp(TextWriter writer)
        {
            optionGroups.WriteOptionDescriptions(writer);
        }

        public Task Execute(string[] commandLineArguments)
        {
            return Task.Run(() =>
            {
                optionGroups.Parse(commandLineArguments);

                if (string.IsNullOrWhiteSpace(id))
                    throw new CommandException("An ID is required");

                if (includes.All(string.IsNullOrWhiteSpace))
                    includes.Add("**");

                if (string.IsNullOrWhiteSpace(basePath))
                    basePath = Path.GetFullPath(Directory.GetCurrentDirectory());

                if (string.IsNullOrWhiteSpace(outFolder))
                    outFolder = Path.GetFullPath(Directory.GetCurrentDirectory());

                if (version == null)
                {
                    var now = DateTime.Now;
                    version = SemanticVersion.Parse($"{now.Year}.{now.Month}.{now.Day}.{now.Hour*10000 + now.Minute*100 + now.Second}");
                }

                if (authors.All(string.IsNullOrWhiteSpace))
                    authors.Add(Environment.GetEnvironmentVariable("USERNAME") + "@" + Environment.GetEnvironmentVariable("USERDOMAIN"));

                if (string.IsNullOrWhiteSpace(description))
                    description = "A deployment package created from files on disk.";

                string allReleaseNotes = null;
                if (!string.IsNullOrWhiteSpace(releaseNotesFile))
                {
                    if (!File.Exists(releaseNotesFile))
                        log.Warning("The release notes file '{Path:l}' could not be found", releaseNotesFile);
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

                if (string.IsNullOrWhiteSpace(version.OriginalString))
                {
                    throw new Exception("Somehow we created a SemanticVersion without the OriginalString value being preserved. We want to use the OriginalString so we can preserve the version as intended by the caller.");
                }

                var metadata = new ManifestMetadata
                {
                    Id = id,
                    Authors = authors,
                    Description = description,
                    Version = NuGetVersion.Parse(version.OriginalString)
                };

                if (!string.IsNullOrWhiteSpace(allReleaseNotes))
                    metadata.ReleaseNotes = allReleaseNotes;

                if (!string.IsNullOrWhiteSpace(title))
                    metadata.Title = title;

                log.Information($"{(verbose ? "Verbose logging" : "")}");
                log.Information("Packing {id:l} version {Version}...", id, version);

                packageBuilder.BuildPackage(basePath, includes, metadata, outFolder, overwrite, verbose);

                log.Information("Done.");
            });
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
