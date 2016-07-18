using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using NuGet.Packaging;
using NuGet.Versioning;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using PackageBuilder = Octopus.Cli.Commands.NuGet.PackageBuilder;

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
            var package = new PackageBuilder();

            package.PopulateFiles(basePath, includes.Select(i => new ManifestFile { Source = i }));
            package.Populate(metadata);

            var filename = metadata.Id + "." + GetNormalizedVersionForFileName(metadata.Version)  + ".nupkg";
            var output = Path.Combine(outFolder, filename);

            if (fileSystem.FileExists(output) && !overwrite)
                throw new CommandException("The package file already exists and --overwrite was not specified");

            log.InfoFormat("Saving {0} to {1}...", filename, outFolder);

            fileSystem.EnsureDirectoryExists(outFolder);

            using (var outStream = fileSystem.OpenFile(output, FileMode.Create))
                package.Save(outStream);
        }

        static string GetNormalizedVersionForFileName(NuGetVersion specifiedVersion)
        {
            var normalized = specifiedVersion.ToNormalizedString();

            if (specifiedVersion.HasMetadata)
            {
                normalized = normalized.Replace("+" + specifiedVersion.Metadata, "");
            }

            return normalized;
        }
    }
}