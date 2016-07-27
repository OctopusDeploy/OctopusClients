using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using NuGet.Packaging;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands
{
    public class NuGetPackageBuilder : IPackageBuilder
    {
        readonly IOctopusFileSystem fileSystem;
        readonly ILog log;

        public NuGetPackageBuilder(IOctopusFileSystem fileSystem, ILog log)
        {
            this.fileSystem = fileSystem;
            this.log = log;
        }

        public void BuildPackage(string basePath, IList<string> includes, ManifestMetadata metadata, string outFolder, bool overwrite)
        {
            var nugetPkgBuilder = new NuGet.PackageBuilder();

            nugetPkgBuilder.PopulateFiles(basePath, includes.Select(i => new ManifestFile { Source = i }));
            nugetPkgBuilder.Populate(metadata);

            var filename = metadata.Id + "." + metadata.Version + ".nupkg";
            var output = Path.Combine(outFolder, filename);

            if (fileSystem.FileExists(output) && !overwrite)
                throw new CommandException("The package file already exists and --overwrite was not specified");

            log.InfoFormat("Saving {0} to {1}...", filename, outFolder);

            fileSystem.EnsureDirectoryExists(outFolder);

            using (var outStream = fileSystem.OpenFile(output, FileMode.Create))
                nugetPkgBuilder.Save(outStream);
        }
    }
}
