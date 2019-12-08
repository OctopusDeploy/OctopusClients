using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, bool replaceExisting, bool useDeltaCompression);
        Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, OverwriteMode overwriteMode);
        Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, OverwriteMode overwriteMode, bool useDeltaCompression);

        Task<ResourceCollection<PackageFromBuiltInFeedResource>> ListPackages(string packageId, int skip = 0, int take = 30);
        Task<ResourceCollection<PackageFromBuiltInFeedResource>> LatestPackages(int skip = 0, int take = 30);
        Task DeletePackage(PackageResource package);
        Task DeletePackages(IReadOnlyList<PackageResource> packages);

        Task<PackageFromBuiltInFeedResource> GetPackage(string packageId);
    }

    class BuiltInPackageRepositoryRepository : IBuiltInPackageRepositoryRepository
    {
        private readonly IOctopusAsyncRepository repository;
        private static readonly ILog Logger = LogProvider.For<BuiltInPackageRepositoryRepository>();

        public BuiltInPackageRepositoryRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, OverwriteMode overwriteMode)
        {
            return await PushPackage(fileName, contents, overwriteMode, useDeltaCompression: true);
        }

        public async Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, bool replaceExisting = false)
        {
            return await PushPackage(fileName, contents, replaceExisting ? OverwriteMode.OverwriteExisting : OverwriteMode.FailIfExists, useDeltaCompression: true);
        }

        public async Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, bool replaceExisting, bool useDeltaCompression)
        {
            return await PushPackage(fileName, contents, replaceExisting ? OverwriteMode.OverwriteExisting : OverwriteMode.FailIfExists, useDeltaCompression);
        }

        public async Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, OverwriteMode overwriteMode, bool useDeltaCompression)
        {
            if (useDeltaCompression)
            {
                try
                {
                    var deltaResult = await AttemptDeltaPush(fileName, contents, overwriteMode).ConfigureAwait(false);
                    if (deltaResult != null)
                        return deltaResult;
                }
                catch (TimeoutException ex)
                {
                    Logger.Info("Delta push timed out: " + ex.Message);

                    var verificationResult = await VerifyTransfer(fileName, contents).ConfigureAwait(false);
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

            var link = await repository.Link("PackageUpload").ConfigureAwait(false);
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

            contents.Seek(0, SeekOrigin.Begin);

            try
            {
                return await repository.Client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                    link,
                    new FileUpload() {Contents = contents, FileName = fileName},
                    pathParameters).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                var verificationResult = await VerifyTransfer(fileName, contents);
                if (verificationResult != null) return verificationResult;

                throw;
            }
        }

        private async Task<PackageFromBuiltInFeedResource> VerifyTransfer(string fileName, Stream contents)
        {
            Logger.Info("Trying to find out whether the transfer worked");

            if (!PackageIdentityParser.TryParsePackageIdAndVersion(Path.GetFileNameWithoutExtension(fileName),
                out var packageId, out var version))
            {
                Logger.Info("Can't check whether the transfer actually worked");
                return null;
            }

            var uploadedPackage = await TryFindPackage(packageId, version);

            return PackageContentComparer.AreSame(uploadedPackage, contents, Logger) ? uploadedPackage : null;
        }

        private async Task<PackageFromBuiltInFeedResource> TryFindPackage(string packageId, SemanticVersion version)
        {
            try
            {
                return await repository.BuiltInPackageRepository.GetPackage($"{packageId}.{version}").ConfigureAwait(false);
            }
            catch (OctopusResourceNotFoundException)
            {
                return null;
            }
        }

        private async Task<PackageFromBuiltInFeedResource> AttemptDeltaPush(string fileName, Stream contents, OverwriteMode overwriteMode)
        {
            if (! await repository.HasLink("PackageDeltaSignature").ConfigureAwait(false))
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
                signatureResult = await repository.Client.Get<PackageSignatureResource>(await repository.Link("PackageDeltaSignature").ConfigureAwait(false), new {packageId, version}).ConfigureAwait(false);
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
                    var link = await repository.Link("PackageDeltaUpload").ConfigureAwait(false);
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

                    var result = await repository.Client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                        link,
                        new FileUpload() {Contents = delta, FileName = Path.GetFileName(fileName)},
                        pathParameters).ConfigureAwait(false);

                    Logger.Info("Delta transfer completed");

                    return result;
                }
            }
        }

        public async Task<ResourceCollection<PackageFromBuiltInFeedResource>> ListPackages(string packageId, int skip = 0, int take = 30)
        {
            return await repository.Client.List<PackageFromBuiltInFeedResource>(await repository.Link("Packages").ConfigureAwait(false), new { nuGetPackageId = packageId, take, skip }).ConfigureAwait(false);
        }

        public async Task<PackageFromBuiltInFeedResource> GetPackage(string packageId)
        {
            return await repository.Client.Get<PackageFromBuiltInFeedResource>(await repository.Link("Packages").ConfigureAwait(false), new { id = packageId }).ConfigureAwait(false);
        }

        public async Task<ResourceCollection<PackageFromBuiltInFeedResource>> LatestPackages(int skip = 0, int take = 30)
        {
            return await repository.Client.List<PackageFromBuiltInFeedResource>(await repository.Link("Packages").ConfigureAwait(false), new { latest = true, take, skip }).ConfigureAwait(false);
        }

        public async Task DeletePackage(PackageResource package)
        {
            await repository.Client.Delete(await repository.Link("Packages").ConfigureAwait(false), new { id = package.Id }).ConfigureAwait(false);
        }

        public async Task DeletePackages(IReadOnlyList<PackageResource> packages)
            => await repository.Client.Delete(await repository.Link("PackagesBulk").ConfigureAwait(false), new { ids = packages.Select(p => p.Id).ToArray() }).ConfigureAwait(false);

    }
}
