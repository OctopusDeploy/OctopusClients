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

        public OctopusPackageMetadataGetResource Get(string id)
        {
            var rootDocument = repository.Client.Repository.LoadRootDocument();
            return repository.Client.Get<OctopusPackageMetadataGetResource>(rootDocument.Links["PackageMetadata"], new { id });
        }

        public OctopusPackageMetadataGetResource Push(string packageId, string version, OctopusPackageMetadata octopusMetadata)
        {
            if (string.IsNullOrWhiteSpace(packageId))
                throw new ArgumentException("A package Id must be supplied", nameof(packageId));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("A version must be supplied", nameof(version));
                    
            var resource = new OctopusPackageMetadataPostResource
            {
                PackageId = packageId,
                Version = version,
                OctopusPackageMetadata = octopusMetadata
            };

            var rootDocument = repository.Client.Repository.LoadRootDocument();
            return repository.Client.Post<OctopusPackageMetadataPostResource, OctopusPackageMetadataGetResource>(rootDocument.Links["PackageMetadata"], resource);
        }
    }

    public interface IPackageMetadataRepository
    {
        OctopusPackageMetadataGetResource Get(string id);
        OctopusPackageMetadataGetResource Push(string packageId, string version, OctopusPackageMetadata octopusMetadata);
    }
}