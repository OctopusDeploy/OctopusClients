using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, bool replaceExisting, bool useDeltaCompression);
        PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, OverwriteMode overwriteMode);
        PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, OverwriteMode overwriteMode, bool useDeltaCompression);
        ResourceCollection<PackageFromBuiltInFeedResource> ListPackages(string packageId, int skip = 0, int take = 30);
        ResourceCollection<PackageFromBuiltInFeedResource> LatestPackages(int skip = 0, int take = 30);
        void DeletePackage(PackageResource package);
        void DeletePackages(IReadOnlyList<PackageResource> packages);
        PackageFromBuiltInFeedResource GetPackage(string packageId, string version);
    }

    class BuiltInPackageRepositoryRepository : IBuiltInPackageRepositoryRepository
    {
        private readonly IOctopusRepository repository;
        private static readonly ILog Logger = LogProvider.For<BuiltInPackageRepositoryRepository>();

        public BuiltInPackageRepositoryRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, OverwriteMode overwriteMode)
        {
            return PushPackage(fileName, contents, overwriteMode, useDeltaCompression: true);
        }

        public PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, bool replaceExisting = false)
        {
            return PushPackage(fileName, contents, replaceExisting ? OverwriteMode.OverwriteExisting : OverwriteMode.FailIfExists, useDeltaCompression: true);
        }

        public PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, bool replaceExisting, bool useDeltaCompression)
        {
            return PushPackage(fileName, contents, replaceExisting ? OverwriteMode.OverwriteExisting : OverwriteMode.FailIfExists, useDeltaCompression);
        }

        public PackageFromBuiltInFeedResource GetPackage(string packageId, string version)
        {
            return repository.Client.Get<PackageFromBuiltInFeedResource>(repository.Link("Packages"), new { id = $"{packageId}.{version}" });
        }


        public PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, OverwriteMode overwriteMode, bool useDeltaCompression)
        {
            if (useDeltaCompression)
            {
                try
                {
                    var deltaResult = AttemptDeltaPush(fileName, contents, overwriteMode);
                    if (deltaResult != null)
                        return deltaResult;
                }
                catch (TimeoutException ex)
                {
                    Logger.Info("Delta push timed out: " + ex.Message);

                    var verificationResult = VerifyTransfer(fileName, contents);
                    if (verificationResult != null) return verificationResult;
                }
                catch (Exception ex) when (!(ex is OctopusValidationException))
                {
                    Logger.Info("Something went wrong while performing a delta transfer: " + ex.Message);
                }

                Logger.Info("Falling back to pushing the complete package to the server");
            }
            else
            {
                Logger.Info("Pushing the complete package to the server, as delta compression was explicitly disabled");
            }

            contents.Seek(0, SeekOrigin.Begin);

            var link = repository.Link("PackageUpload");
            object pathParameters;

            // if the link contains overwriteMode then we're connected to a new server, if not use the old `replace` parameter
            if (link.Contains(OverwriteModeLink.Link))
            {
                pathParameters = new { overwriteMode = overwriteMode };
            }
            else
            {
                pathParameters = new { replace = overwriteMode.ConvertToLegacyReplaceFlag(Logger) };
            }


            try
            {
                return repository.Client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                    link,
                    new FileUpload() { Contents = contents, FileName = fileName },
                    pathParameters);
            }
            catch (TimeoutException)
            {
                var verificationResult = VerifyTransfer(fileName, contents);
                if (verificationResult != null) return verificationResult;

                throw;
            }
        }

        private PackageFromBuiltInFeedResource VerifyTransfer(string fileName, Stream contents)
        {
            Logger.Info("Trying to find out whether the transfer worked");

            if (!PackageIdentityParser.TryParsePackageIdAndVersion(Path.GetFileNameWithoutExtension(fileName),
                out var packageId, out var version))
            {
                Logger.Info("Can't check whether the transfer actually worked");
                return null;
            }

            var uploadedPackage = TryFindPackage(packageId, version);

            return PackageContentComparer.AreSame(uploadedPackage, contents, Logger) ? uploadedPackage : null;
        }

        private PackageFromBuiltInFeedResource TryFindPackage(string packageId, SemanticVersion version)
        {
            try
            {
                return repository.BuiltInPackageRepository.GetPackage(packageId, version.ToString());
            }
            catch (OctopusResourceNotFoundException)
            {
                return null;
            }
        }

        private PackageFromBuiltInFeedResource AttemptDeltaPush(string fileName, Stream contents, OverwriteMode overwriteMode)
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
                signatureResult = repository.Client.Get<PackageSignatureResource>(repository.Link("PackageDeltaSignature"), new {packageId, version});
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
                    var link = repository.Link("PackageDeltaUpload");
                    object pathParameters;

                    // if the link contains overwriteMode then we're connected to a new server, if not use the old `replace` parameter
                    if (link.Contains(OverwriteModeLink.Link))
                    {
                        pathParameters = new { overwriteMode = overwriteMode, packageId, signatureResult.BaseVersion };
                    }
                    else
                    {
                        pathParameters = new { replace = overwriteMode.ConvertToLegacyReplaceFlag(Logger), packageId, signatureResult.BaseVersion };
                    }

                    var result = repository.Client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                        link,
                        new FileUpload() {Contents = delta, FileName = Path.GetFileName(fileName)},
                        pathParameters);

                    Logger.Info($"Delta transfer completed");
                    return result;
                }
            }
        }

        public ResourceCollection<PackageFromBuiltInFeedResource> ListPackages(string packageId, int skip = 0, int take = 30)
        {
            return repository.Client.List<PackageFromBuiltInFeedResource>(repository.Link("Packages"), new { nuGetPackageId = packageId, take, skip });
        }

        public ResourceCollection<PackageFromBuiltInFeedResource> LatestPackages(int skip = 0, int take = 30)
        {
            return repository.Client.List<PackageFromBuiltInFeedResource>(repository.Link("Packages"), new { latest = true, take, skip });
        }

        public void DeletePackage(PackageResource package)
        {
            repository.Client.Delete(repository.Link("Packages"), new { id = package.Id });
        }

        public void DeletePackages(IReadOnlyList<PackageResource> packages)
            => repository.Client.Delete(repository.Link("PackagesBulk"), new { ids = packages.Select(p => p.Id).ToArray() });
    }
}