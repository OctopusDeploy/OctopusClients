using System;
using System.Threading.Tasks;
using Octopus.Client.Logging;
using Octopus.Client.Model;
using Octopus.Client.Model.PackageMetadata;

namespace Octopus.Client.Repositories.Async
{
    class PackageMetadataRepository : IPackageMetadataRepository
    {
        private readonly IOctopusAsyncRepository repository;
        private static readonly ILog Logger = LogProvider.For<PackageMetadataRepository>();

        public PackageMetadataRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public async Task<OctopusPackageMetadataMappedResource> Get(string id)
        {
            var link = await repository.Link("PackageMetadata");
            return await repository.Client.Get<OctopusPackageMetadataMappedResource>(link, new { id });
        }

        public Task<OctopusPackageMetadataMappedResource> Push(string packageId, string version, OctopusPackageMetadata octopusMetadata, bool replaceExisting)
        {
            return Push(packageId, version, octopusMetadata, replaceExisting ? OverwriteMode.OverwriteExisting : OverwriteMode.FailIfExists);
        }
        
        public async Task<OctopusPackageMetadataMappedResource> Push(string packageId, string version, OctopusPackageMetadata octopusMetadata, OverwriteMode overwriteMode)
        {
            if (string.IsNullOrWhiteSpace(packageId))
                throw new ArgumentException("A package Id must be supplied", nameof(packageId));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("A version must be supplied", nameof(version));

            var resource = new OctopusPackageMetadataVersionResource
            {
                PackageId = packageId,
                Version = version,
                OctopusPackageMetadata = octopusMetadata
            };

            var link = await repository.Link("PackageMetadata");

            // if the link contains overwriteMode then we're connected to a new server, if not use the old `replace` parameter  
            if (link.Contains(OverwriteModeLink.Link))
            {
                return await repository.Client.Post<OctopusPackageMetadataVersionResource, OctopusPackageMetadataMappedResource>(link, resource, new { overwriteMode = overwriteMode });
            }
            else
            {
                return await repository.Client.Post<OctopusPackageMetadataVersionResource, OctopusPackageMetadataMappedResource>(link, resource, new { replace = overwriteMode.AsLegacyReplaceFlag(Logger) });
            }
        }
    }

    public interface IPackageMetadataRepository
    {
        Task<OctopusPackageMetadataMappedResource> Get(string id);
        Task<OctopusPackageMetadataMappedResource> Push(string packageId, string version, OctopusPackageMetadata octopusMetadata, OverwriteMode overwriteMode);
        [Obsolete]
        Task<OctopusPackageMetadataMappedResource> Push(string packageId, string version, OctopusPackageMetadata octopusMetadata, bool replaceExisting);
    }
}