using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Packaging;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands.Package
{
    public class NuGetPackageBuilder : IPackageBuilder
    {
        readonly IOctopusFileSystem fileSystem;
        private readonly ICommandOutputProvider commandOutputProvider;
        private readonly List<string> files;

        public NuGetPackageBuilder(IOctopusFileSystem fileSystem, ICommandOutputProvider commandOutputProvider)
        {
            this.fileSystem = fileSystem;
            this.commandOutputProvider = commandOutputProvider;
            files = new List<string>();
        }

        public string[] Files => files.ToArray();
        public string PackageFormat => "nupkg";

        public void BuildPackage(string basePath, IList<string> includes, ManifestMetadata metadata, string outFolder, bool overwrite, bool verboseInfo)
        {
            var nugetPkgBuilder = new NuGet.Packaging.PackageBuilder();

            var manifestFiles = includes.Select(i => new ManifestFile {Source = i}).ToList();
            nugetPkgBuilder.PopulateFiles(basePath, manifestFiles);
            nugetPkgBuilder.Populate(metadata);

            if (verboseInfo)
            {
                foreach(var file in nugetPkgBuilder.Files)
                {
                    commandOutputProvider.Information($"Added file: {file.Path}");
                }
            }
            files.AddRange(nugetPkgBuilder.Files.Select(x=>x.Path).ToArray());

            var filename = $"{metadata.Id}.{metadata.Version}.nupkg";
            var output = Path.Combine(outFolder, filename);

            if (fileSystem.FileExists(output) && !overwrite)
                throw new CommandException("The package file already exists and --overwrite was not specified");

            commandOutputProvider.Information("Saving {Filename} to {OutFolder}...", filename, outFolder);

            fileSystem.EnsureDirectoryExists(outFolder);

            using (var outStream = fileSystem.OpenFile(output, FileMode.Create))
                nugetPkgBuilder.Save(outStream);
        }
    }
}