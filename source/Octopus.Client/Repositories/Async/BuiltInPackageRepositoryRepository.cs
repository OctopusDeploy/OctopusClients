using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Octodiff.Core;
using Octodiff.Diagnostics;
using Octopus.Client.Exceptions;
using Octopus.Client.Features;
using Octopus.Client.Logging;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface IBuiltInPackageRepositoryRepository
    {
        Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, bool replaceExisting = false);
        Task<ResourceCollection<PackageFromBuiltInFeedResource>> ListPackages(string packageId, int skip = 0, int take = 30);
        Task<ResourceCollection<PackageFromBuiltInFeedResource>> LatestPackages(int skip = 0, int take = 30);
        Task DeletePackage(PackageResource package);
        Task DeletePackages(IReadOnlyList<PackageResource> packages);
    }

    class BuiltInPackageRepositoryRepository : IBuiltInPackageRepositoryRepository
    {
        private readonly IOctopusAsyncRepository repository;
        private static readonly ILog Logger = LogProvider.For<BuiltInPackageRepositoryRepository>();

        public BuiltInPackageRepositoryRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, bool replaceExisting = false)
        {

            try
            {
                var deltaResult = await AttemptDeltaPush(fileName, contents, replaceExisting);
                if (deltaResult != null)
                    return deltaResult;
            }
            catch(Exception ex) when (!(ex is OctopusValidationException))
            {
                Logger.Info("Something went wrong while performing a delta transfer: " + ex.Message);
            }

            
            Logger.Info("Falling back to pushing the complete package to the server");
                
            contents.Seek(0, SeekOrigin.Begin);
            var result = await repository.Client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                repository.Link("PackageUpload"),
                new FileUpload() {Contents = contents, FileName = fileName},
                new {replace = replaceExisting});
                
            Logger.Info("Package transfer completed");

            return result;
        }

        private async Task<PackageFromBuiltInFeedResource> AttemptDeltaPush(string fileName, Stream contents, bool replaceExisting)
        {
            if (!repository.HasLink("PackageDeltaSignature"))
            {
                Logger.Info("Server does not support delta compression for package push");
                return null;
            }

            if (!PackageIdentityParser.TryParsePackageIdAndVersion(Path.GetFileNameWithoutExtension(fileName), out var packageId, out var version))
            {
                Logger.Info("Could not determine the package ID and/or version based on the supplied filename");
                return null;
            }
            
            PackageSignatureResource signatureResult;
            try
            {
                Logger.Info($"Requesting signature for delta compression from the server for upload of a package with id '{packageId}' and version '{version}'");
                signatureResult = await repository.Client.Get<PackageSignatureResource>(repository.Link("PackageDeltaSignature"), new {packageId, version});
            }
            catch (OctopusResourceNotFoundException)
            {
                Logger.Info("No package with the same ID exists on the server");
                return null;
            }
                
            using(var deltaTempFile = new TemporaryFile())
            {
                var shouldUpload = DeltaCompression.CreateDelta(contents, signatureResult, deltaTempFile.FileName);
                if (!shouldUpload)
                    return null;
                
                using (var delta = File.OpenRead(deltaTempFile.FileName))
                {
                    var result = await repository.Client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                        repository.Link("PackageDeltaUpload"),
                        new FileUpload() {Contents = delta, FileName = Path.GetFileName(fileName)},
                        new {replace = replaceExisting, packageId, signatureResult.BaseVersion});

                    Logger.Info($"Delta transfer completed");
                    return result;
                }
            }
        }

        public Task<ResourceCollection<PackageFromBuiltInFeedResource>> ListPackages(string packageId, int skip = 0, int take = 30)
        {
            return repository.Client.List<PackageFromBuiltInFeedResource>(repository.Link("Packages"), new { nuGetPackageId = packageId, take, skip });
        }

        public Task<ResourceCollection<PackageFromBuiltInFeedResource>> LatestPackages(int skip = 0, int take = 30)
        {
            return repository.Client.List<PackageFromBuiltInFeedResource>(repository.Link("Packages"), new { latest = true, take, skip });
        }

        public Task DeletePackage(PackageResource package)
        {
            return repository.Client.Delete(repository.Link("Packages"), new { id = package.Id });
        }

        public Task DeletePackages(IReadOnlyList<PackageResource> packages)
            => repository.Client.Delete(repository.Link("PackagesBulk"), new { ids = packages.Select(p => p.Id).ToArray() });
        
    }
}
