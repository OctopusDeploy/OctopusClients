using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Logging;
using Octopus.Client.Model;
using Octopus.Client.Model.BuildInformation;

namespace Octopus.Client.Repositories.Async
{
    class BuildInformationRepository : IBuildInformationRepository
    {
        private readonly IOctopusAsyncRepository repository;
        private static readonly ILog Logger = LogProvider.For<BuildInformationRepository>();

        public BuildInformationRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public async Task<OctopusBuildInformationMappedResource> Get(string id)
        {
            var link = await repository.Link("BuildInformation");
            return await repository.Client.Get<OctopusBuildInformationMappedResource>(link, new { id });
        }

        public Task<OctopusBuildInformationMappedResource> Push(string packageId, string version, OctopusBuildInformation octopusMetadata, bool replaceExisting)
        {
            return Push(packageId, version, octopusMetadata, replaceExisting ? OverwriteMode.OverwriteExisting : OverwriteMode.FailIfExists);
        }
        
        public async Task<OctopusBuildInformationMappedResource> Push(string packageId, string version, OctopusBuildInformation octopusMetadata, OverwriteMode overwriteMode)
        {
            if (string.IsNullOrWhiteSpace(packageId))
                throw new ArgumentException("A package Id must be supplied", nameof(packageId));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("A version must be supplied", nameof(version));

            var resource = new OctopusBuildInformationVersionResource
            {
                PackageId = packageId,
                Version = version,
                OctopusBuildInformation = octopusMetadata
            };

            if (!(await repository.HasLink("BuildInformation")))
            {
                throw new OperationNotSupportedByOctopusServerException(
                    OctopusBuildInformation.BuildInformationRequiresOctopusVersionMessage,
                    OctopusBuildInformation.BuildInformationRequiresOctopusVersion);
            }

            var link = await repository.Link("BuildInformation");

            return await repository.Client.Post<OctopusBuildInformationVersionResource, OctopusBuildInformationMappedResource>(link, resource, new { overwriteMode = overwriteMode });
        }
        
        public async Task<ResourceCollection<OctopusBuildInformationMappedResource>> ListBuilds(string packageId, int skip = 0, int take = 30)
        {
            return await repository.Client.List<OctopusBuildInformationMappedResource>(await repository.Link("BuildInformation").ConfigureAwait(false), new { packageId = packageId, take, skip }).ConfigureAwait(false);
        }

        public async Task<ResourceCollection<OctopusBuildInformationMappedResource>> LatestBuilds(int skip = 0, int take = 30)
        {
            return await repository.Client.List<OctopusBuildInformationMappedResource>(await repository.Link("BuildInformation").ConfigureAwait(false), new { latest = true, take, skip }).ConfigureAwait(false);
        }

        public async Task Delete(OctopusBuildInformationMappedResource buildInformation)
        {
            await repository.Client.Delete(await repository.Link("BuildInformation").ConfigureAwait(false), new { id = buildInformation.Id }).ConfigureAwait(false);
        }

        public async Task DeleteBuilds(IReadOnlyList<OctopusBuildInformationMappedResource> builds)
            => await repository.Client.Delete(await repository.Link("BuildInformationBulk").ConfigureAwait(false), new { ids = builds.Select(p => p.Id).ToArray() }).ConfigureAwait(false);
    }

    public interface IBuildInformationRepository
    {
        Task<OctopusBuildInformationMappedResource> Get(string id);
        Task<OctopusBuildInformationMappedResource> Push(string packageId, string version, OctopusBuildInformation octopusMetadata, OverwriteMode overwriteMode);
        [Obsolete]
        Task<OctopusBuildInformationMappedResource> Push(string packageId, string version, OctopusBuildInformation octopusMetadata, bool replaceExisting);

        Task<ResourceCollection<OctopusBuildInformationMappedResource>> ListBuilds(string packageId, int skip = 0, int take = 30);
        Task<ResourceCollection<OctopusBuildInformationMappedResource>> LatestBuilds(int skip = 0, int take = 30);
        Task Delete(OctopusBuildInformationMappedResource buildInformation);
        Task DeleteBuilds(IReadOnlyList<OctopusBuildInformationMappedResource> builds);
    }
}