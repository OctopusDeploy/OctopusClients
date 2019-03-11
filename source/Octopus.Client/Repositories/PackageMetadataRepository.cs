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
            var rootDocument = repository.Client.Repository.LoadRootDocument();
            return repository.Client.Get<OctopusPackageMetadataMappedResource>(rootDocument.Links["PackageMetadata"], new { id });
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
                OctopusPackageMetadata = octopusMetadata
            };

            var rootDocument = repository.Client.Repository.LoadRootDocument();
            return repository.Client.Post<OctopusPackageMetadataVersionResource, OctopusPackageMetadataMappedResource>(rootDocument.Links["PackageMetadata"], resource);
        }
    }

    public interface IPackageMetadataRepository
    {
        OctopusPackageMetadataMappedResource Get(string id);
        OctopusPackageMetadataMappedResource Push(string packageId, string version, OctopusPackageMetadata octopusMetadata);
    }
}