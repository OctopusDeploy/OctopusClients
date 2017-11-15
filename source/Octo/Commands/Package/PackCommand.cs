using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Packaging;
using NuGet.Versioning;
using Octopus.Cli.Diagnostics;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Model;
using Octopus.Cli.Util;
using Serilog;
using SemanticVersion = Octopus.Client.Model.SemanticVersion;

namespace Octopus.Cli.Commands.Package
{
    [Command("pack", Description = "Creates a package (.nupkg or .zip) from files on disk, without needing a .nuspec or .csproj")]
    public class PackCommand : CommandBase, ICommand, ISupportFormattedOutput
    {
        readonly IList<string> authors = new List<string>();
        readonly IOctopusFileSystem fileSystem;
        readonly IList<string> includes = new List<string>();
        string basePath;
        string description;
        string id;
        string outFolder;
        bool overwrite;
        bool verbose;
        string releaseNotes, releaseNotesFile;
        string title;
        SemanticVersion version;
        IPackageBuilder packageBuilder;
        string allReleaseNotes;

        public PackCommand(IOctopusFileSystem fileSystem, ICommandOutputProvider commandOutputProvider) : base(commandOutputProvider)
        {
            this.fileSystem = fileSystem;

            var common = Options.For("Advanced options");
            common.Add("include=", "[Optional, Multiple] Add a file pattern to include, relative to the base path e.g. /bin/*.dll - if none are specified, defaults to **", v => includes.Add(v));
            common.Add("overwrite", "[Optional] Allow an existing package file of the same ID/version to be overwritten", v => overwrite = true);

            var nuget = Options.For("NuGet packages");
            nuget.Add("author=", "[Optional, Multiple] Add an author to the package metadata; defaults to the current user", v => authors.Add(v));
            nuget.Add("title=", "[Optional] The title of the package", v => title = v);
            nuget.Add("description=", "[Optional] A description of the package; defaults to a generic description", v => description = v);
            nuget.Add("releaseNotes=", "[Optional] Release notes for this version of the package", v => releaseNotes = v);
            nuget.Add("releaseNotesFile=", "[Optional] A file containing release notes for this version of the package", v => releaseNotesFile = v);
            
            var basic = Options.For("Basic options");
            basic.Add("id=", "The ID of the package; e.g. MyCompany.MyApp", v => id = v);
            basic.Add("format=", "Package format. Options are: NuPkg, Zip. Defaults to NuPkg, though we recommend Zip going forward", fmt => packageBuilder = SelectFormat(fmt));
            basic.Add("version=", "[Optional] The version of the package; must be a valid SemVer; defaults to a timestamp-based version", v => version = string.IsNullOrWhiteSpace(v) ? null : new SemanticVersion(v));
            basic.Add("outFolder=", "[Optional] The folder into which the generated NUPKG file will be written; defaults to '.'", v => { v.CheckForIllegalPathCharacters(nameof(outFolder)); outFolder = v;});
            basic.Add("basePath=", "[Optional] The root folder containing files and folders to pack; defaults to '.'", v => { v.CheckForIllegalPathCharacters(nameof(basePath)); basePath = v;});
            basic.Add("verbose", "[Optional] verbose output", v => verbose = true);
            basic.AddLogLevelOptions();

            packageBuilder = SelectFormat("nupkg");
        }

       public Task Execute(string[] commandLineArguments)
        {
            return Task.Run(() =>
            {
                Options.Parse(commandLineArguments);

                if (printHelp)
                {
                    this.GetHelp(Console.Out, commandLineArguments);
                    return;
                }

                commandOutputProvider.PrintMessages = this.OutputFormat == OutputFormat.Default || this.verbose;

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
                    authors.Add(System.Environment.GetEnvironmentVariable("USERNAME") + "@" + System.Environment.GetEnvironmentVariable("USERDOMAIN"));

                if (string.IsNullOrWhiteSpace(description))
                    description = "A deployment package created from files on disk.";

                allReleaseNotes = null;
                if (!string.IsNullOrWhiteSpace(releaseNotesFile))
                {
                    if (!File.Exists(releaseNotesFile))
                        commandOutputProvider.Warning("The release notes file '{Path:l}' could not be found", releaseNotesFile);
                    else
                        allReleaseNotes = fileSystem.ReadFile(releaseNotesFile);
                }

                if (!string.IsNullOrWhiteSpace(releaseNotes))
                {
                    if (allReleaseNotes != null)
                        allReleaseNotes += System.Environment.NewLine + releaseNotes;
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

                
                if (verbose)
                    commandOutputProvider.Information("Verbose logging");
                commandOutputProvider.Information("Packing {id:l} version {Version}...", id, version);

                packageBuilder.BuildPackage(basePath, includes, metadata, outFolder, overwrite, verbose);

                if (OutputFormat == OutputFormat.Json)
                {
                    PrintJsonOutput();
                }
                else
                {
                    PrintDefaultOutput();
                }
            });
        }

        IPackageBuilder SelectFormat(string fmt)
        {
            switch (fmt.ToLowerInvariant())
            {
                case "zip":
                    return new ZipPackageBuilder(fileSystem, commandOutputProvider);
                case "nupkg":
                case "nuget":
                    return new NuGetPackageBuilder(fileSystem, commandOutputProvider);
                default:
                    throw new CommandException("Unknown package format: " + fmt);
            }
        }

        public Task Request()
        {
            return Task.WhenAny();
        }

        public void PrintDefaultOutput()
        {
            commandOutputProvider.Information("Done.");
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(new
            {
                PackageId = this.id,
                Version = this.version.ToString(),
                ReleaseNotes = allReleaseNotes ?? string.Empty,
                Description = this.description,
                packageBuilder.PackageFormat,
                OutputFolder = this.outFolder,
                Files = packageBuilder.Files.Any() ? packageBuilder.Files : includes,
            });   
        }
    }
}
