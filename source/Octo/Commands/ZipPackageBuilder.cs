using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NuGet.Common;
using NuGet.Packaging;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Serilog.Events;

namespace Octopus.Cli.Commands
{
    public class ZipPackageBuilder : IPackageBuilder
    {
        readonly IOctopusFileSystem fileSystem;
        readonly Serilog.ILogger log;

        public ZipPackageBuilder(IOctopusFileSystem fileSystem, Serilog.ILogger log)
        {
            this.fileSystem = fileSystem;
            this.log = log;
        }

        public void BuildPackage(string basePath, IList<string> includes, ManifestMetadata metadata, string outFolder, bool overwrite, bool verboseInfo)
        {
            var filename = metadata.Id + "." + metadata.Version + ".zip";
            var output = fileSystem.GetFullPath(Path.Combine(outFolder, filename));

            if (fileSystem.FileExists(output) && !overwrite)
                throw new CommandException("The package file already exists and --overwrite was not specified");

            log.Information("Saving {Filename} to {OutFolder}...", filename, outFolder);

            fileSystem.EnsureDirectoryExists(outFolder);

            var basePathLength = fileSystem.GetFullPath(basePath).Length;
            using (var stream = fileSystem.OpenFile(output, FileAccess.Write))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                foreach (var pattern in includes)
                {
                    log.Debug("Adding files from {Path} matching pattern {Pattern}", basePath, pattern);
                    foreach (var file in PathResolver.PerformWildcardSearch(basePath, pattern))
                    {
                        var fullFilePath = fileSystem.GetFullPath(file);
                        if (string.Equals(fullFilePath, output, StringComparison.CurrentCultureIgnoreCase))
                            continue;

                        var relativePath = UseCrossPlatformDirectorySeparator(
                            fullFilePath.Substring(basePathLength).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                        if (verboseInfo)
                            log.Information($"Added file: {relativePath}");

                        var entry = archive.CreateEntry(relativePath, CompressionLevel.Optimal);
                        entry.LastWriteTime = new DateTimeOffset(new FileInfo(file).LastWriteTime);

                        using (var entryStream = entry.Open())
                        using (var sourceStream = File.OpenRead(file))
                        {
                            sourceStream.CopyTo(entryStream);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Per the .ZIP File Format Specification 4.4.17.1 all slashes should be forward slashes, not back slashes. 
        /// https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT
        ///
        /// This functionality is being implemented in the framework: 
        /// https://github.com/dotnet/corefx/commit/7b9331e89a795c72709aef38898929e74c343dfb
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static string UseCrossPlatformDirectorySeparator(string path)
        {
            if (Path.DirectorySeparatorChar == '\\')
                return path.Replace('\\', '/');

            return path;
        }
    }
}