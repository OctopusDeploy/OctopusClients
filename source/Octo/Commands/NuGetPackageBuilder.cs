using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Packaging;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands
{
    public class NuGetPackageBuilder : IPackageBuilder
    {
        readonly IOctopusFileSystem fileSystem;
        readonly Serilog.ILogger log;

        public NuGetPackageBuilder(IOctopusFileSystem fileSystem, Serilog.ILogger log)
        {
            this.fileSystem = fileSystem;
            this.log = log;
        }

        public void BuildPackage(string basePath, IList<string> includes, ManifestMetadata metadata, string outFolder, bool overwrite, bool verboseInfo)
        {
            var nugetPkgBuilder = new PackageBuilder();

            var manifestFiles = includes.Select(i => new ManifestFile {Source = i}).ToList();
            nugetPkgBuilder.PopulateFiles(basePath, manifestFiles);
            nugetPkgBuilder.Populate(metadata);

            if (verboseInfo)
            {
                nugetPkgBuilder.Files.ToList().ForEach(f => log.Information("Adding file: {Path}", f.Path));
            }

            var filename = metadata.Id + "." + metadata.Version + ".nupkg";
            var output = Path.Combine(outFolder, filename);

            if (fileSystem.FileExists(output) && !overwrite)
                throw new CommandException("The package file already exists and --overwrite was not specified");

            log.Information("Saving {Filename} to {OutFolder}...", filename, outFolder);

            fileSystem.EnsureDirectoryExists(outFolder);

            using (var outStream = fileSystem.OpenFile(output, FileMode.Create))
                nugetPkgBuilder.Save(outStream);
        }
    }
}