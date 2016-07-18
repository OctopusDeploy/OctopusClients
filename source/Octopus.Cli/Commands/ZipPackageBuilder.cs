using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using log4net;
using NuGet.Common;
using NuGet.Packaging;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands
{
    public class ZipPackageBuilder : IPackageBuilder
    {
        readonly IOctopusFileSystem fileSystem;
        readonly ILog log;

        public ZipPackageBuilder(IOctopusFileSystem fileSystem, ILog log)
        {
            this.fileSystem = fileSystem;
            this.log = log;
        }

        public void BuildPackage(string basePath, IList<string> includes, ManifestMetadata metadata, string outFolder, bool overwrite)
        {
            var filename = metadata.Id + "." + metadata.Version + ".zip";
            var output = fileSystem.GetFullPath(Path.Combine(outFolder, filename));

            if (fileSystem.FileExists(output) && !overwrite)
                throw new CommandException("The package file already exists and --overwrite was not specified");

            log.InfoFormat("Saving {0} to {1}...", filename, outFolder);

            fileSystem.EnsureDirectoryExists(outFolder);

            var basePathLength = fileSystem.GetFullPath(basePath).Length;
            using (var stream = fileSystem.OpenFile(output, FileAccess.Write))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                foreach (var pattern in includes)
                {
                    log.DebugFormat("Adding files from '{0}' matching pattern '{1}'", basePath, pattern);
                    foreach (var file in PathResolver.PerformWildcardSearch(basePath, pattern))
                    {
                        var fullFilePath = fileSystem.GetFullPath(file);
                        if (string.Equals(fullFilePath, output, StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        var relativePath = fullFilePath.Substring(basePathLength).TrimStart('\\');
                        log.Debug("Adding file: " + relativePath);

                        var entry = archive.CreateEntry(relativePath, CompressionLevel.Optimal);
                        entry.LastWriteTime = new DateTimeOffset(new FileInfo(file).LastWriteTimeUtc);
                        using (var entryStream = entry.Open())
                        using (var sourceStream = File.OpenRead(file))
                        {
                            sourceStream.CopyTo(entryStream);
                        }
                    }
                }
            }
        }
    }
}