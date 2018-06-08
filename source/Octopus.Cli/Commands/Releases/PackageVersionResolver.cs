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
    /// <summary>
    /// Represents the package that a version applies to
    /// </summary>
    class PackageKey : IEqualityComparer<PackageKey>
    {
        public string StepNameOrPackageId { get; }
        public string PackageReferenceName { get; }

        public PackageKey()
        {

        }

        public PackageKey(string stepNameOrPackageId)
        {
            StepNameOrPackageId = stepNameOrPackageId;
        }

        public PackageKey(string stepNameOrPackageId, string packageReferenceName)
        {
            StepNameOrPackageId = stepNameOrPackageId;
            PackageReferenceName = packageReferenceName;
        }

        public bool Equals(PackageKey x, PackageKey y)
        {
            return Object.Equals(x, y);
        }

        public int GetHashCode(PackageKey obj)
        {
            return obj.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var key = obj as PackageKey;
            return key != null &&
                   string.Equals(StepNameOrPackageId, key.StepNameOrPackageId, StringComparison.CurrentCultureIgnoreCase) &&
                   string.Equals(PackageReferenceName, key.PackageReferenceName, StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            var hashCode = 475932885;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StepNameOrPackageId?.ToLower());
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PackageReferenceName?.ToLower());
            return hashCode;
        }
    }

    public class PackageVersionResolver : IPackageVersionResolver
    {
        /// <summary>
        /// Used to indicate a match with any matching step name or package reference name
        /// </summary>
        private const string WildCard = "*";
        static readonly string[] SupportedZipFilePatterns = { "*.zip", "*.tgz", "*.tar.gz", "*.tar.Z", "*.tar.bz2", "*.tar.bz", "*.tbz", "*.tar" };

        readonly ILogger log;
        private readonly IOctopusFileSystem fileSystem;
        readonly IDictionary<PackageKey, string> stepNameToVersion = new Dictionary<PackageKey, string>(new PackageKey());
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

                if (TryReadPackageIdentity(file, out var packageIdentity))
                {
                    Add(packageIdentity.Id, null, packageIdentity.Version.ToString());
                }
            }
            foreach (var file in fileSystem.EnumerateFilesRecursively(folderPath, SupportedZipFilePatterns))
            {
                log.Debug("Package file: {File:l}", file);

                if (TryParseZipIdAndVersion(file, out var packageIdentity))
                {
                    Add(packageIdentity.Id, null, packageIdentity.Version.ToString());
                }
            }
        }

        public void Add(string stepNameAndVersion)
        {
            var split = stepNameAndVersion.Split(':', '=', '/');
            if (split.Length < 2)
                throw new CommandException("The package argument '" + stepNameAndVersion + "' does not use expected format of : {Step Name}:{Version}");

            var stepOrPackageId = split[0];
            var packageReferenceName = split.Length > 2 ? split[1] : null;
            var version = split.Length > 2 ? split[2] : split[1];

            if (string.IsNullOrWhiteSpace(stepOrPackageId) || string.IsNullOrWhiteSpace(version))
            {
                throw new CommandException("The package argument '" + stepNameAndVersion + "' does not use expected format of : {Step Name}:{Version}");
            }

            if (!SemanticVersion.TryParse(version, out var parsedVersion))
            {
                throw new CommandException("The version portion of the package constraint '" + stepNameAndVersion + "' is not a valid semantic version number.");
            }

            Add(stepOrPackageId, packageReferenceName, version);
        }

        public void Add(string stepName, string packageVersion)
        {
            Add(stepName, null, packageVersion);
        }

        public void Add(string stepName, string packageReferenceName, string packageVersion)
        {
            // Double wild card == default value
            if (stepName == WildCard && packageReferenceName == WildCard)
            {
                Default(packageVersion);
                return;
            }

            var key = new PackageKey(stepName, packageReferenceName);
            if (stepNameToVersion.TryGetValue(key, out var current))
            {
                var newVersion = SemanticVersion.Parse(packageVersion);
                var currentVersion = SemanticVersion.Parse(current);
                if (newVersion < currentVersion)
                {
                    return;
                }
            }

            stepNameToVersion[key] = packageVersion;
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
            return ResolveVersion(stepName, packageId, null);
        }

        public string ResolveVersion(string stepName, string packageId, string packageReferenceName)
        {
            var identifiers = new[] {stepName, packageId};

            // First attempt to get an exact match between step or package id and the package reference name
            return identifiers
                    .Select(id => new PackageKey(id, packageReferenceName))
                    .Select(key => stepNameToVersion.TryGetValue(key, out var version) ? version : null)
                    .FirstOrDefault(version => version != null)
                ??
                // If that fails, try to match on a wildcard step/package id and exact package reference name,
                // and then on an exact step/package id and wildcard package reference name
                identifiers
                    .SelectMany(id => new[]
                        {new PackageKey(WildCard, packageReferenceName), new PackageKey(id, WildCard)})
                    .Select(key => stepNameToVersion.TryGetValue(key, out var version) ? version : null)
                    .FirstOrDefault(version => version != null)
                ??
                // Finally, use the default version
                defaultVersion;
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
