using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using NuGet;
using NuGet.Versioning;
using Octopus.Cli.Infrastructure;
using SemanticVersion = Octopus.Client.Model.SemanticVersion;

namespace Octopus.Cli.Commands
{
    public class PackageVersionResolver : IPackageVersionResolver
    {
        readonly ILog log;
        readonly IDictionary<string, string> stepNameToVersion = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        string defaultVersion;

        public PackageVersionResolver(ILog log)
        {
            this.log = log;
        }

        public void AddFolder(string folderPath)
        {
            log.Debug("Using package versions from folder: " + folderPath);
            foreach (var file in Directory.GetFiles(folderPath, "*.nupkg", SearchOption.AllDirectories))
            {
                log.Debug("Package file: " + file);
                var package = new ZipPackage(file);

                Add(package.Id, package.Version.ToString());
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

            NuGetVersion version;
            if (!NuGetVersion.TryParse(value, out version))
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
                var newVersion = NuGetVersion.Parse(packageVersion);
                var currentVersion = NuGetVersion.Parse(current);
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
                NuGetVersion.Parse(packageVersion);
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
    }
}
