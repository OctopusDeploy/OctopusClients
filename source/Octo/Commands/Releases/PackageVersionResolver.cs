using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Serilog;
using SemanticVersion = Octopus.Client.Model.SemanticVersion;

namespace Octopus.Cli.Commands.Releases
{
    public class PackageVersionResolver : IPackageVersionResolver
    {
        static readonly string[] SupportedZipFilePatterns = { "*.zip", "*.tgz", "*.tar.gz", "*.tar.Z", "*.tar.bz2", "*.tar.bz", "*.tbz", "*.tar" };

        readonly ILogger log;
        private readonly IOctopusFileSystem fileSystem;
        readonly IDictionary<string, string> stepNameToVersion = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        string defaultVersion;

        public PackageVersionResolver(Serilog.ILogger log, IOctopusFileSystem fileSystem)
        {
            this.log = log;
            this.fileSystem = fileSystem;
        }

        public void AddFolder(string folderPath)
        {
            log.Debug("Using package versions from folder: {FolderPath:l}", folderPath);
            foreach (var file in fileSystem.EnumerateFilesRecursively(folderPath, "*.nupkg"))
            {
                log.Debug("Package file: {File:l}", file);

                PackageIdentity packageIdentity;
                if (TryReadPackageIdentity(file, out packageIdentity))
                {
                    Add(packageIdentity.Id, packageIdentity.Version.ToString());
                }
            }
            foreach (var file in fileSystem.EnumerateFilesRecursively(folderPath, SupportedZipFilePatterns))
            {
                log.Debug("Package file: {File:l}", file);

                PackageIdentity packageIdentity;
                if (TryParseZipIdAndVersion(file, out packageIdentity))
                {
                    Add(packageIdentity.Id, packageIdentity.Version.ToString());
                }
            }
        }

        public void Add(string stepNameAndVersion)
        {
            var index = new[] {':', '='}.Select(s => stepNameAndVersion.IndexOf(s)).Where(i => i >= 0).OrderBy(i => i).FirstOrDefault();
            if (index <= 0)
                throw new CommandException("The package argument '" + stepNameAndVersion + "' does not use expected format of : {Step Name}:{Version}");

            var key = stepNameAndVersion.Substring(0, index);
            var value = (index >= stepNameAndVersion.Length - 1) ? string.Empty : stepNameAndVersion.Substring(index + 1);

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                throw new CommandException("The package argument '" + stepNameAndVersion + "' does not use expected format of : {Step Name}:{Version}");
            }

            SemanticVersion version;
            if (!SemanticVersion.TryParse(value, out version))
            {
                throw new CommandException("The version portion of the package constraint '" + stepNameAndVersion + "' is not a valid semantic version number.");
            }

            Add(key, value);
        }

        public void Add(string stepName, string packageVersion)
        {
            string current;
            if (stepNameToVersion.TryGetValue(stepName, out current))
            {
                var newVersion = SemanticVersion.Parse(packageVersion);
                var currentVersion = SemanticVersion.Parse(current);
                if (newVersion < currentVersion)
                {
                    return;
                }
            }

            stepNameToVersion[stepName] = packageVersion;
        }

        public void Default(string packageVersion)
        {
            try
            {
                SemanticVersion.Parse(packageVersion);
                defaultVersion = packageVersion;
            }
            catch (ArgumentException)
            {
                if (packageVersion.Contains(":"))
                {
                    throw new ArgumentException("Invalid package version format. Use the package parameter if you need to specify the step name and version.");
                }
                throw;
            }
        }

        public string ResolveVersion(string stepName, string packageId)
        {
             string version;
             if (stepNameToVersion.TryGetValue(stepName, out version))
                 return version;
           
             if (stepNameToVersion.TryGetValue(packageId, out version))
                 return version;
           
            return defaultVersion;
        }

        bool TryReadPackageIdentity(string packageFile, out PackageIdentity packageIdentity)
        {
            packageIdentity = null;
            try
            {
                using (var reader = new PackageArchiveReader(new FileStream(packageFile, FileMode.Open, FileAccess.Read)))
                {
                    var nuspecReader = new NuspecReader(reader.GetNuspec());
                    packageIdentity = nuspecReader.GetIdentity();
                    return true;
                }
            }
            catch (Exception ex)
            {
               log.Warning(ex, "Could not read manifest from '{PackageFile:l}'", packageFile); 
            }

            return false;
        }

        /// <summary>
        /// Takes a string containing a concatenated package ID and version (e.g. a filename or database-key) and 
        /// attempts to parse a package ID and semantic version.  
        /// </summary>
        /// <param name="filename">The filename of the package</param>
        /// <param name="packageIdentity">The package identity</param>
        /// <returns>True if parsing was successful, else False</returns>
        static bool TryParseZipIdAndVersion(string filename, out PackageIdentity packageIdentity)
        {
            packageIdentity = null;

            var idAndVersion = Path.GetFileNameWithoutExtension(filename) ?? "";
            if (".tar".Equals(Path.GetExtension(idAndVersion), StringComparison.OrdinalIgnoreCase))
                idAndVersion = Path.GetFileNameWithoutExtension(idAndVersion);

            const string packageIdPattern = @"(?<packageId>(\w+([_.-]\w+)*?))";
            const string semanticVersionPattern = @"(?<semanticVersion>(\d+(\.\d+){0,3}" // Major Minor Patch
                 + @"(-[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?)" // Pre-release identifiers
                 + @"(\+[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?)"; // Build Metadata

            var match = Regex.Match(idAndVersion, $@"^{packageIdPattern}\.{semanticVersionPattern}$");
            var packageIdMatch = match.Groups["packageId"];
            var versionMatch = match.Groups["semanticVersion"];

            if (!packageIdMatch.Success || !versionMatch.Success)
                return false;

            var packageId = packageIdMatch.Value;

            NuGetVersion version;
            if (!NuGetVersion.TryParse(versionMatch.Value, out version))
                return false;

            packageIdentity = new PackageIdentity(packageId, version);
            return true;
        }
    }
}
