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
using Serilog;

namespace Octopus.Cli.Commands.Package
{
    [Command("push", Description = "Pushes a package (.nupkg, .zip, .tar.gz, etc.) package to the built-in NuGet repository in an Octopus server.")]
    public class PushCommand : ApiCommand, ISupportFormattedOutput
    {
        private List<string> pushedPackages;
        private List<Tuple<string, Exception>> failedPackages;

        public PushCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Package pushing");
            options.Add("package=", "Package file to push. Specify multiple packages by specifying this argument multiple times: \n--package package1 --package package2", package => Packages.Add(EnsurePackageExists(fileSystem, package)));
            options.Add("replace-existing", "If the package already exists in the repository, the default behavior is to reject the new package being pushed. You can pass this flag to overwrite the existing package.", replace => ReplaceExisting = true);

            pushedPackages = new List<string>();
            failedPackages = new List<Tuple<string, Exception>>();
        }

        public HashSet<string> Packages { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase); 
        public bool ReplaceExisting { get; set; }

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

        public void PrintDefaultOutput()
        {
            commandOutputProvider.Debug("Push successful");
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(new
            {
                SuccessfulPackages = pushedPackages,
                FailedPackages = failedPackages.Select(x=>new { Package = x.Item1, Reason = x.Item2.Message.Replace("\r\n", string.Empty) })
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