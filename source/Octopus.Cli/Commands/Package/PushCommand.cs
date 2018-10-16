using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Model;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Util;
using Serilog;

namespace Octopus.Cli.Commands.Package
{
    [Command("push", Description = "Pushes a package (.nupkg, .zip, .tar.gz, etc.) package to the built-in NuGet repository in an Octopus server.")]
    public class PushCommand : ApiCommand, ISupportFormattedOutput
    {
        private List<string> pushedPackages;
        private List<string> existingPackages;
        private List<Tuple<string, Exception>> failedPackages;

        public PushCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Package pushing");
            options.Add("package=", "Package file to push. Specify multiple packages by specifying this argument multiple times: \n--package package1 --package package2", package => Packages.Add(EnsurePackageExists(fileSystem, package)));
            options.Add("replace-existing", "If the package already exists in the repository, the default behavior is to reject the new package being pushed. You can pass this flag to overwrite the existing package.", replace => ReplaceExisting = true);
            options.Add("ignore-existing", "If the package already exists in the repository, do nothing (a success will return)", replace => IgnoreExiting = true);

            pushedPackages = new List<string>();
            existingPackages = new List<string>();
            failedPackages = new List<Tuple<string, Exception>>();
        }

        public HashSet<string> Packages { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase); 
        public bool ReplaceExisting { get; set; }
        public bool IgnoreExiting { get; set; }

        public async Task Request()
        {
            if (Packages.Count == 0) throw new CommandException("Please specify a package to push");

            foreach (var package in Packages)
            {
                commandOutputProvider.Debug("Pushing package: {Package:l}...", package);

                try
                {
                    using (var fileStream = FileSystem.OpenFile(package, FileAccess.Read))
                    {
                        await Repository.BuiltInPackageRepository
                            .PushPackage(Path.GetFileName(package), fileStream, ReplaceExisting).ConfigureAwait(false);
                    }

                    pushedPackages.Add(package);
                }
                catch (OctopusValidationException ex) when (IgnoreExiting && ex.HttpStatusCode == 409)
                {
                    var packageExists = await CheckIfPackageExists(package);
                    if (packageExists)
                    {
                        commandOutputProvider.Debug("Package already exists in repository: {Package:l}", package);
                        existingPackages.Add(package);
                    }
                    else if (OutputFormat == OutputFormat.Default)
                    {
                        throw;
                    }
                    failedPackages.Add(new Tuple<string, Exception>(package, ex));
                }
                catch (Exception ex)
                {
                    if (OutputFormat == OutputFormat.Default)
                    {
                        throw;
                    }
                    failedPackages.Add(new Tuple<string, Exception>(package, ex));
                }
            }
        }

        private async Task<bool> CheckIfPackageExists(string package)
        {
            if (!PackageIdentityParser.TryParsePackageIdAndVersion(Path.GetFileNameWithoutExtension(package), out var packageId, out var version))
            {
                return false;
            }

            var packages = new List<PackageFromBuiltInFeedResource>();
            var moreToGet = true;
            try
            {
                while (moreToGet)
                {
                    var retrieved = await Repository.BuiltInPackageRepository.ListPackages(packageId, packages.Count).ConfigureAwait(false);
                    packages.AddRange(retrieved.Items);
                    if (retrieved.TotalResults == packages.Count)
                    {
                        moreToGet = false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return packages.Any(x => x.Version == version.ToString());
        }

        public void PrintDefaultOutput()
        {
            commandOutputProvider.Debug("Push successful");
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(new
            {
                SuccessfulPackages = pushedPackages,
                AlreadyExistingPackage = existingPackages,
                FailedPackages = failedPackages.Select(x => new { Package = x.Item1, Reason = x.Item2.Message.Replace("\r\n", string.Empty) })
            });
        }

        static string EnsurePackageExists(IOctopusFileSystem fileSystem, string package)
        {
            var path = fileSystem.GetFullPath(package);
            if (!File.Exists(path)) throw new CommandException("Package file not found: " + path);
            return path;
        }
    }
}