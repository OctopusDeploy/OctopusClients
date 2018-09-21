using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Octopus.Client.Exceptions;
using Octopus.Client.Features;
using Octopus.Client.Logging;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    public interface IBuiltInPackageRepositoryRepository
    {
        PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, bool replaceExisting = false);
        ResourceCollection<PackageFromBuiltInFeedResource> ListPackages(string packageId, int skip = 0, int take = 30);
        ResourceCollection<PackageFromBuiltInFeedResource> LatestPackages(int skip = 0, int take = 30);
        void DeletePackage(PackageResource package);
        void DeletePackages(IReadOnlyList<PackageResource> packages);
    }

    class BuiltInPackageRepositoryRepository : IBuiltInPackageRepositoryRepository
    {
        readonly IOctopusClient client;
        private static readonly ILog Logger = LogProvider.For<BuiltInPackageRepositoryRepository>();

        public BuiltInPackageRepositoryRepository(IOctopusClient client)
        {
            this.client = client;
        }

        public PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, bool replaceExisting = false)
        {
            try
            {
                var deltaResult = AttemptDeltaPush(fileName, contents, replaceExisting);
                if (deltaResult != null)
                    return deltaResult;
            }
            catch(Exception ex) when (!(ex is OctopusValidationException))
            {
                Logger.Info("Something went wrong while performing a delta transfer: " + ex.Message);
            }

            
            Logger.Info("Falling back to pushing the complete package to the server");
                
            contents.Seek(0, SeekOrigin.Begin);
            
            var result = client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                client.Link("PackageUpload"),
                new FileUpload() { Contents = contents, FileName = fileName },
                new { replace = replaceExisting });
            
            Logger.Info("Package transfer completed");

            return result;
        }
        
        private PackageFromBuiltInFeedResource AttemptDeltaPush(string fileName, Stream contents, bool replaceExisting)
        {
            if (!client.HasLink("PackageDeltaSignature"))
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
                signatureResult = client.Get<PackageSignatureResource>(client.Link("PackageDeltaSignature"), new {packageId, version});
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
                    var result = client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                        client.Link("PackageDeltaUpload"),
                        new FileUpload() {Contents = delta, FileName = Path.GetFileName(fileName)},
                        new {replace = replaceExisting, packageId, signatureResult.BaseVersion});

                    Logger.Info($"Delta transfer completed");
                    return result;
                }
            }
        }

        public ResourceCollection<PackageFromBuiltInFeedResource> ListPackages(string packageId, int skip = 0, int take = 30)
        {
            return client.List<PackageFromBuiltInFeedResource>(client.Link("Packages"), new { nuGetPackageId = packageId, take, skip });
        }

        public ResourceCollection<PackageFromBuiltInFeedResource> LatestPackages(int skip = 0, int take = 30)
        {
            return client.List<PackageFromBuiltInFeedResource>(client.Link("Packages"), new { latest = true, take, skip });
        }

        public void DeletePackage(PackageResource package)
        {
            client.Delete(client.Link("Packages"), new { id = package.Id });
        }

        public void DeletePackages(IReadOnlyList<PackageResource> packages)
            => client.Delete(client.Link("PackagesBulk"), new { ids = packages.Select(p => p.Id).ToArray() });
    }
}