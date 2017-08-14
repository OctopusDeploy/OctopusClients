using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octopus.Cli.Commands
{
    [Command("push", Description = "Pushes a package (.nupkg, .zip, .tar.gz, etc.) package to the built-in NuGet repository in an Octopus server.")]
    public class PushCommand : ApiCommand
    {
        public PushCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, log, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Package pushing");
            options.Add("package=", "Package file to push. Specify multiple packages by specifying this argument multiple times: \n--package package1 --package package2", package => Packages.Add(EnsurePackageExists(fileSystem, package)));
            options.Add("replace-existing", "If the package already exists in the repository, the default behavior is to reject the new package being pushed. You can pass this flag to overwrite the existing package.", replace => ReplaceExisting = true);
        }

        public HashSet<string> Packages { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase); 
        public bool ReplaceExisting { get; set; }

        protected override async Task Execute()
        {
            if (Packages.Count == 0) throw new CommandException("Please specify a package to push");

            foreach (var package in Packages)
            {
                Log.Debug("Pushing package: {Package:l}...", package);

                using (var fileStream = FileSystem.OpenFile(package, FileAccess.Read))
                {
                    await Repository.BuiltInPackageRepository.PushPackage(Path.GetFileName(package), fileStream, ReplaceExisting).ConfigureAwait(false);
                }
            }

            Log.Debug("Push successful");
        }

        static string EnsurePackageExists(IOctopusFileSystem fileSystem, string package)
        {
            var path = fileSystem.GetFullPath(package);
            if (!File.Exists(path)) throw new CommandException("Package file not found: " + path);
            return path;
        }
    }
}