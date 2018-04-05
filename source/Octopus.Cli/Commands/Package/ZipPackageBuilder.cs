using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NuGet.Common;
using NuGet.Packaging;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands.Package
{
    public class ZipPackageBuilder : IPackageBuilder
    {
        readonly IOctopusFileSystem fileSystem;
        private readonly ICommandOutputProvider commandOutputProvider;
        private readonly List<string> files;

        public ZipPackageBuilder(IOctopusFileSystem fileSystem, ICommandOutputProvider commandOutputProvider)
        {
            this.fileSystem = fileSystem;
            this.commandOutputProvider = commandOutputProvider;
            files = new List<string>();
        }

        public string[] Files => files.ToArray();

        public string PackageFormat => "zip";

        public void BuildPackage(string basePath, IList<string> includes, ManifestMetadata metadata, string outFolder, bool overwrite, bool verboseInfo)
        {
            var filename = metadata.Id + "." + metadata.Version + ".zip";
            var output = fileSystem.GetFullPath(Path.Combine(outFolder, filename));

            if (fileSystem.FileExists(output) && !overwrite)
                throw new CommandException("The package file already exists and --overwrite was not specified");

            commandOutputProvider.Information("Saving {Filename} to {OutFolder}...", filename, outFolder);

            fileSystem.EnsureDirectoryExists(outFolder);

            var basePathLength = fileSystem.GetFullPath(basePath).Length;
            using (var stream = fileSystem.OpenFile(output, FileAccess.Write))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                foreach (var pattern in includes)
                {
                    commandOutputProvider.Debug("Adding files from {Path} matching pattern {Pattern}", basePath, pattern);
                    foreach (var file in PathResolver.PerformWildcardSearch(basePath, pattern))
                    {
                        var fullFilePath = fileSystem.GetFullPath(file);
                        if (string.Equals(fullFilePath, output, StringComparison.CurrentCultureIgnoreCase))
                            continue;

                        var relativePath = UseCrossPlatformDirectorySeparator(
                            fullFilePath.Substring(basePathLength).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                        if (verboseInfo)
                        {
                            commandOutputProvider.Information($"Added file: {relativePath}");
                        }

                        files.Add(relativePath);

                        var entry = archive.CreateEntry(relativePath, CompressionLevel.Optimal);
                        UpdateLastWriteTime(file, entry);

                        using (var entryStream = entry.Open())
                        using (var sourceStream = File.OpenRead(file))
                        {
                            sourceStream.CopyTo(entryStream);
                        }
                    }
                }
            }
        }

        private static readonly DateTime MinDatetime = new DateTime(1980, 1, 1);
        private static readonly DateTime MaxDatetime = new DateTime(2107, 12, 31, 23, 59, 58);

        private static void UpdateLastWriteTime(string file, ZipArchiveEntry entry)
        {
            var fileInfo = new FileInfo(file);
            
            try
            {
                entry.LastWriteTime = new DateTimeOffset(fileInfo.LastWriteTime);
            }
            catch (ArgumentOutOfRangeException ex)
                when (ex.Message.StartsWith("The DateTimeOffset specified cannot be converted into a Zip file timestamp."))
            {
                // This occurs if the file LastWriteTime is missing, resulting in the OS reading it as UNIX min
                // System.Compression.IO requires the date to be between 1980/01/01 00:00 and 2107/12/31 23:59:58
                // https://github.com/dotnet/corefx/blob/master/src/System.IO.Compression/src/System/IO/Compression/ZipArchiveEntry.cs#L216

                entry.LastWriteTime = new DateTimeOffset(fileInfo.LastWriteTime > MinDatetime ? MaxDatetime : MinDatetime, TimeSpan.Zero);
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