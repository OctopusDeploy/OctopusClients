using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.PackageMetadata;

namespace Octopus.Client.Repositories.Async
{
    public class PackageMetadataRepository : IPackageMetadataRepository
    {
        private readonly IOctopusAsyncRepository repository;

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

            // if the link doesn't contain overwritemode then we're connected to an older server, which uses the `replace` parameter  
            if (link.Contains(OverwriteModeLink.Link))
            {
                return await repository.Client.Post<OctopusPackageMetadataVersionResource, OctopusPackageMetadataMappedResource>(link, resource, new { overwrite = overwriteMode });
            }
            else
            {
                return await repository.Client.Post<OctopusPackageMetadataVersionResource, OctopusPackageMetadataMappedResource>(link, resource, new { replace = overwriteMode == OverwriteMode.OverwriteExisting });
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