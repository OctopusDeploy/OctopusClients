using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands
{
    [Command("push", Description = "Pushes a package (.nupkg, .zip, .tar.gz, etc.) package to the built-in NuGet repository in an Octopus server.")]
    public class PushCommand : ApiCommand
    {
        public PushCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Package pushing");
            options.Add("package=", "Package file to push. Specify multiple packages by specifying this argument multiple times: \n--package package1 --package package2", package => Packages.Add(EnsurePackageExists(fileSystem, package)));
            options.Add("replace-existing", "If the package already exists in the repository, the default behavior is to reject the new package being pushed. You can pass this flag to overwrite the existing package.", replace => ReplaceExisting = true);
        }

        public HashSet<string> Packages { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase); 
        public bool ReplaceExisting { get; set; }

        protected override void Execute()
        {
            if (Packages.Count == 0) throw new CommandException("Please specify a package to push");

            foreach (var package in Packages)
            {
                Log.DebugFormat("Pushing package: {0}...", package);

                using (var fileStream = FileSystem.OpenFile(package, FileAccess.Read))
                {
                    Repository.BuiltInPackageRepository.PushPackage(Path.GetFileName(package), fileStream, ReplaceExisting);
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