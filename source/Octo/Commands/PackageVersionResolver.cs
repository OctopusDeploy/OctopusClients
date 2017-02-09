using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Model.Versioning;
using SemanticVersion = Octopus.Client.Model.SemanticVersion;

namespace Octopus.Cli.Commands
{
    public class PackageVersionResolver : IPackageVersionResolver
    {
        readonly Serilog.ILogger log;
        readonly IDictionary<string, string> stepNameToVersion = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        string defaultVersion;
        readonly Regex zipNameRegex = new Regex(@"(?<Name>.+?)\.(?<Version>\d+(\s*\.\s*\d+){0,3})(?<Release>-[a-z][0-9a-z-]*)?\.zip$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        public PackageVersionResolver(Serilog.ILogger log)
        {
            this.log = log;
        }

        public void AddFolder(string folderPath)
        {
            log.Debug("Using package versions from folder: {FolderPath:l}", folderPath);
            foreach (var file in Directory.GetFiles(folderPath, "*.nupkg", SearchOption.AllDirectories))
            {
                log.Debug("Package file: {File:l}", file);

                PackageIdentity packageIdentity;
                if (TryReadPackageIdentity(file, out packageIdentity))
                {
                    Add(packageIdentity.Id, packageIdentity.Version.ToString());
                }
            }
            foreach (var file in Directory.GetFiles(folderPath, "*.zip", SearchOption.AllDirectories))
            {
                log.Debug("Package file: {File:l}", file);

                PackageIdentity packageIdentity;
                if (TryReadZipIdentity(file, out packageIdentity))
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

        public string ResolveVersion(string stepName)
        {
            string version;
            return stepNameToVersion.TryGetValue(stepName, out version)
                ? version
                : defaultVersion;
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

        bool TryReadZipIdentity(string zipFile, out PackageIdentity packageIdentity)
        {
            try
            {
                string fileName = Path.GetFileName(zipFile);
                var match = zipNameRegex.Match(fileName);
                if (match.Success)
                {
                    string packageId = match.Groups["Name"].Value;
                    string version = match.Groups["Version"].Value;
                    string release = string.Empty;
                    if (match.Groups["Release"] != null)
                    {
                        release = match.Groups["Release"].Value;
                    }

                    packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(version + release));
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Warning(ex, "Could not determine package name and version from zip file '{PackageFile:l}'", zipFile);
            }

            packageIdentity = null;
            return false;
        }
    }
}
