using System;
using System.Threading.Tasks;
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

        public async Task<OctopusPackageMetadataMappedResource> Push(string packageId, string version, OctopusPackageMetadata octopusMetadata, bool replaceExisting)
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
            return await repository.Client.Post<OctopusPackageMetadataVersionResource, OctopusPackageMetadataMappedResource>(link, resource, new { replace = replaceExisting });
        }
    }

    public interface IPackageMetadataRepository
    {
        Task<OctopusPackageMetadataMappedResource> Get(string id);
        Task<OctopusPackageMetadataMappedResource> Push(string packageId, string version, OctopusPackageMetadata octopusMetadata, bool replaceExisting);
    }
}