using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Exceptions;
using Octopus.Client.Logging;
using Octopus.Client.Model;
using Octopus.Client.Model.BuildInformation;

namespace Octopus.Client.Repositories
{
    public class BuildInformationRepository : IBuildInformationRepository
    {
        private readonly IOctopusRepository repository;
        private static readonly ILog Logger = LogProvider.For<BuildInformationRepository>();

        public BuildInformationRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public OctopusBuildInformationMappedResource Get(string id)
        {
            var link = repository.Link("BuildInformation");
            return repository.Client.Get<OctopusBuildInformationMappedResource>(link, new { id });
        }

        public OctopusBuildInformationMappedResource Push(string packageId, string version, OctopusBuildInformation octopusMetadata)
        {
            return Push(packageId, version, octopusMetadata, OverwriteMode.FailIfExists);
        }

        public OctopusBuildInformationMappedResource Push(string packageId, string version, OctopusBuildInformation octopusMetadata, OverwriteMode overwriteMode)
        {
            if (string.IsNullOrWhiteSpace(packageId))
                throw new ArgumentException("A package Id must be supplied", nameof(packageId));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("A version must be supplied", nameof(version));
                    
            var resource = new OctopusBuildInformationVersionResource
            {
                PackageId = packageId,
                Version = version,
                OctopusBuildInformation = octopusMetadata,
            };

            if (!repository.HasLink("BuildInformation"))
            {
                throw new OperationNotSupportedByOctopusServerException(
                    OctopusBuildInformation.BuildInformationRequiresOctopusVersionMessage,
                    OctopusBuildInformation.BuildInformationRequiresOctopusVersion);
            }

            var link = repository.Link("BuildInformation");
            
            return repository.Client.Post<OctopusBuildInformationVersionResource, OctopusBuildInformationMappedResource>(link, resource, new { overwriteMode = overwriteMode });
        }

        public ResourceCollection<OctopusBuildInformationMappedResource> ListBuilds(string packageId, int skip = 0, int take = 30)
        {
            return repository.Client.List<OctopusBuildInformationMappedResource>(repository.Link("BuildInformation"), new { packageId = packageId, take, skip });
        }

        public ResourceCollection<OctopusBuildInformationMappedResource> LatestBuilds(int skip = 0, int take = 30)
        {
            return repository.Client.List<OctopusBuildInformationMappedResource>(repository.Link("BuildInformation"), new { latest = true, take, skip });
        }

        public void Delete(OctopusBuildInformationMappedResource buildInformation)
        {
            repository.Client.Delete(repository.Link("BuildInformation"), new { id = buildInformation.Id });
        }

        public void DeleteBuilds(IReadOnlyList<OctopusBuildInformationMappedResource> builds)
            => repository.Client.Delete(repository.Link("BuildInformationBulk"), new { ids = builds.Select(p => p.Id).ToArray() });
    }

    public interface IBuildInformationRepository
    {
        OctopusBuildInformationMappedResource Get(string id);
        OctopusBuildInformationMappedResource Push(string packageId, string version, OctopusBuildInformation octopusMetadata, OverwriteMode overwriteMode);
        OctopusBuildInformationMappedResource Push(string packageId, string version, OctopusBuildInformation octopusMetadata);
        ResourceCollection<OctopusBuildInformationMappedResource> ListBuilds(string packageId, int skip = 0, int take = 30);
        ResourceCollection<OctopusBuildInformationMappedResource> LatestBuilds(int skip = 0, int take = 30);
        void Delete(OctopusBuildInformationMappedResource buildInformation);
        void DeleteBuilds(IReadOnlyList<OctopusBuildInformationMappedResource> builds);
    }
}