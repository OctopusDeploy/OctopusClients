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

        public OctopusPackageVersionBuildInformationMappedResource Get(string id)
        {
            var link = repository.Link("BuildInformation");
            return repository.Client.Get<OctopusPackageVersionBuildInformationMappedResource>(link, new { id });
        }

        public OctopusPackageVersionBuildInformationMappedResource Push(string packageId, string version, OctopusBuildInformation octopusMetadata)
        {
            return Push(packageId, version, octopusMetadata, OverwriteMode.FailIfExists);
        }

        public OctopusPackageVersionBuildInformationMappedResource Push(string packageId, string version, OctopusBuildInformation octopusMetadata, OverwriteMode overwriteMode)
        {
            if (string.IsNullOrWhiteSpace(packageId))
                throw new ArgumentException("A package Id must be supplied", nameof(packageId));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("A version must be supplied", nameof(version));
                    
            var resource = new OctopusPackageVersionBuildInformationResource
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
            
            return repository.Client.Post<OctopusPackageVersionBuildInformationResource, OctopusPackageVersionBuildInformationMappedResource>(link, resource, new { overwriteMode = overwriteMode });
        }

        public ResourceCollection<OctopusPackageVersionBuildInformationMappedResource> ListBuilds(string packageId, int skip = 0, int take = 30)
        {
            return repository.Client.List<OctopusPackageVersionBuildInformationMappedResource>(repository.Link("BuildInformation"), new { packageId = packageId, take, skip });
        }

        public ResourceCollection<OctopusPackageVersionBuildInformationMappedResource> LatestBuilds(int skip = 0, int take = 30)
        {
            return repository.Client.List<OctopusPackageVersionBuildInformationMappedResource>(repository.Link("BuildInformation"), new { latest = true, take, skip });
        }

        public void Delete(OctopusPackageVersionBuildInformationMappedResource buildInformation)
        {
            repository.Client.Delete(repository.Link("BuildInformation"), new { id = buildInformation.Id });
        }

        public void DeleteBuilds(IReadOnlyList<OctopusPackageVersionBuildInformationMappedResource> builds)
        {
            repository.Client.Delete(repository.Link("BuildInformationBulk"), new {ids = builds.Select(p => p.Id).ToArray()});
        }
    }

    public interface IBuildInformationRepository
    {
        OctopusPackageVersionBuildInformationMappedResource Get(string id);
        OctopusPackageVersionBuildInformationMappedResource Push(string packageId, string version, OctopusBuildInformation octopusMetadata, OverwriteMode overwriteMode);
        OctopusPackageVersionBuildInformationMappedResource Push(string packageId, string version, OctopusBuildInformation octopusMetadata);
        ResourceCollection<OctopusPackageVersionBuildInformationMappedResource> ListBuilds(string packageId, int skip = 0, int take = 30);
        ResourceCollection<OctopusPackageVersionBuildInformationMappedResource> LatestBuilds(int skip = 0, int take = 30);
        void Delete(OctopusPackageVersionBuildInformationMappedResource buildInformation);
        void DeleteBuilds(IReadOnlyList<OctopusPackageVersionBuildInformationMappedResource> builds);
    }
}