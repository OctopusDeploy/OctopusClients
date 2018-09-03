using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Octodiff.Core;
using Octodiff.Diagnostics;
using Octopus.Client.Exceptions;
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
        private static readonly ILog Logger = LogProvider.For<BuiltInPackageRepositoryRepository>();

        readonly IOctopusAsyncClient client;

        public BuiltInPackageRepositoryRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public async Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, bool replaceExisting = false)
        {
            var result = await AttemptDeltaPush(fileName, contents, replaceExisting);
            if (result == null)
            {
                Logger.Info("Falling back to pushing the complete package to the server");

                result = await client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                    client.RootDocument.Link("PackageUpload"),
                    new FileUpload() {Contents = contents, FileName = fileName},
                    new {replace = replaceExisting});
                
                Logger.Info("Package transfer completed");
            }

            return result;
        }

        private async Task<PackageFromBuiltInFeedResource> AttemptDeltaPush(string fileName, Stream contents, bool replaceExisting)
        {
            if (!client.RootDocument.HasLink("PackageDeltaSignature"))
            {
                Logger.Info("Server does not support delta compression for package push");
                return null;
            }

            var deltaTempFile = Path.GetTempFileName();
            try
            {
                if (!PackageIdentityParser.TryParsePackageIdAndVersion(Path.GetFileNameWithoutExtension(fileName), out var packageId, out var version))
                {
                    Logger.Info("Could not determine the package ID and/or version based on the supplied filename");
                    return null;
                }

                Logger.Info($"Requesting signature for delta compression from the server for upload of a package with id '{packageId}' and version '{version}'");
                var signatureResult = await client.Get<PackageSignatureResource>(client.RootDocument.Link("PackageDeltaSignature"), new {packageId, version});

                Logger.Info($"Calculating delta");
                var deltaBuilder = new DeltaBuilder();

                using (var signature = new MemoryStream(signatureResult.Signature))
                using (var deltaStream = File.Open(deltaTempFile, FileMode.Create, FileAccess.ReadWrite))
                {
                    deltaBuilder.BuildDelta(
                        contents,
                        new SignatureReader(signature, new NullProgressReporter()),
                        new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream))
                    );
                }
                
                var originalFileSize = contents.Length;
                var deltaFileSize = new FileInfo(deltaTempFile).Length;
                var ratio = deltaFileSize / (double) originalFileSize;

                if (ratio > 0.95)
                {
                    Logger.Info($"The delta file ({deltaFileSize:n0} bytes) more than 95% the size of the orginal file ({originalFileSize:n0} bytes)");
                    return null;
                }

                Logger.Info($"The delta file ({deltaFileSize:n0} bytes) is {ratio:p2} the size of the orginal file ({originalFileSize:n0} bytes), uploading...");

                using (var delta = File.OpenRead(deltaTempFile))
                {
                    var result = await client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                        client.RootDocument.Link("PackageDeltaUpload"),
                        new FileUpload() {Contents = delta, FileName = Path.GetFileName(fileName)},
                        new {replace = replaceExisting, packageId, signatureResult.BaseVersion});

                    Logger.Info($"Delta transfer completed");
                    return result;
                }
            }
            catch (OctopusResourceNotFoundException)
            {
                Logger.Info("No package with the same ID exists on the server");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Info("Something went wrong while performing a delta transfer: " + ex.Message);
                return null;
            }
            finally
            {
                contents.Seek(0, SeekOrigin.Begin);
                try
                {
                    File.Delete(deltaTempFile);
                }
                catch
                {
                    Logger.Debug("Failed to delete the temporary file");
                }
            }
        }

        public Task<ResourceCollection<PackageFromBuiltInFeedResource>> ListPackages(string packageId, int skip = 0, int take = 30)
        {
            return client.List<PackageFromBuiltInFeedResource>(client.RootDocument.Link("Packages"), new { nuGetPackageId = packageId, take, skip });
        }

        public Task<ResourceCollection<PackageFromBuiltInFeedResource>> LatestPackages(int skip = 0, int take = 30)
        {
            return client.List<PackageFromBuiltInFeedResource>(client.RootDocument.Link("Packages"), new { latest = true, take, skip });
        }

        public Task DeletePackage(PackageResource package)
        {
            return client.Delete(client.RootDocument.Link("Packages"), new { id = package.Id });
        }

        public Task DeletePackages(IReadOnlyList<PackageResource> packages)
            => client.Delete(client.RootDocument.Link("PackagesBulk"), new { ids = packages.Select(p => p.Id).ToArray() });
        
    }
}
