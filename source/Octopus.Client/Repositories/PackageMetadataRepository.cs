using System;
using Octopus.Client.Model.PackageMetadata;

namespace Octopus.Client.Repositories
{
    public class PackageMetadataRepository : IPackageMetadataRepository
    {
        private readonly IOctopusRepository repository;

        public PackageMetadataRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public OctopusPackageMetadataMappedResource Get(string id)
        {
            var link = repository.Link("PackageMetadata");
            return repository.Client.Get<OctopusPackageMetadataMappedResource>(link, new { id });
        }

        public OctopusPackageMetadataMappedResource Push(string packageId, string version, OctopusPackageMetadata octopusMetadata)
        {
            if (string.IsNullOrWhiteSpace(packageId))
                throw new ArgumentException("A package Id must be supplied", nameof(packageId));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("A version must be supplied", nameof(version));
                    
            var resource = new OctopusPackageMetadataVersionResource
            {
                PackageId = packageId,
                Version = version,
                OctopusPackageMetadata = octopusMetadata,
            };

            var link = repository.Link("PackageMetadata");
            return repository.Client.Post<OctopusPackageMetadataVersionResource, OctopusPackageMetadataMappedResource>(link, resource);
        }
    }

    public interface IPackageMetadataRepository
    {
        OctopusPackageMetadataMappedResource Get(string id);
        OctopusPackageMetadataMappedResource Push(string packageId, string version, OctopusPackageMetadata octopusMetadata);
    }
}