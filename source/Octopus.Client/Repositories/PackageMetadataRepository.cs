using System;
using Octopus.Client.Model;
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
            return Push(packageId, version, octopusMetadata, OverwriteMode.FailIfExists);
        }

        public OctopusPackageMetadataMappedResource Push(string packageId, string version, OctopusPackageMetadata octopusMetadata, OverwriteMode overwriteMode)
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
            
            // if the link doesn't contain overwritemode then we're connected to an older server, which uses the `replace` parameter  
            if (link.Contains(OverwriteModeLink.Link))
            {
                return repository.Client.Post<OctopusPackageMetadataVersionResource, OctopusPackageMetadataMappedResource>(link, resource, new { overwriteMode = overwriteMode });
            }
            else
            {
                return repository.Client.Post<OctopusPackageMetadataVersionResource, OctopusPackageMetadataMappedResource>(link, resource, new { replace = overwriteMode == OverwriteMode.OverwriteExisting });
            }
        }
    }

    public interface IPackageMetadataRepository
    {
        OctopusPackageMetadataMappedResource Get(string id);
        OctopusPackageMetadataMappedResource Push(string packageId, string version, OctopusPackageMetadata octopusMetadata, OverwriteMode overwriteMode);
        OctopusPackageMetadataMappedResource Push(string packageId, string version, OctopusPackageMetadata octopusMetadata);
    }
}