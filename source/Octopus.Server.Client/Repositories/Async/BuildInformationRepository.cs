using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public Task<OctopusPackageVersionBuildInformationMappedResource> Get(string id)
            => Get(id, CancellationToken.None);

        public async Task<OctopusPackageVersionBuildInformationMappedResource> Get(string id, CancellationToken cancellationToken)
        {
            var link = await repository.Link("BuildInformation").ConfigureAwait(false);
            return await repository.Client.Get<OctopusPackageVersionBuildInformationMappedResource>(link, new { id }, cancellationToken).ConfigureAwait(false);
        }

        public Task<OctopusPackageVersionBuildInformationMappedResource> Push(string packageId, string version, OctopusBuildInformation octopusMetadata, bool replaceExisting)
        {
            return Push(packageId, version, octopusMetadata, replaceExisting ? OverwriteMode.OverwriteExisting : OverwriteMode.FailIfExists);
        }

        public Task<OctopusPackageVersionBuildInformationMappedResource> Push(string packageId, string version, OctopusBuildInformation octopusMetadata, OverwriteMode overwriteMode)
            => Push(packageId, version, octopusMetadata, overwriteMode, CancellationToken.None);

        public async Task<OctopusPackageVersionBuildInformationMappedResource> Push(string packageId, string version, OctopusBuildInformation octopusMetadata, OverwriteMode overwriteMode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(packageId))
                throw new ArgumentException("A package Id must be supplied", nameof(packageId));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("A version must be supplied", nameof(version));

            if (!(await repository.HasLink("BuildInformation").ConfigureAwait(false)))
            {
                throw new OperationNotSupportedByOctopusServerException(
                    OctopusBuildInformation.BuildInformationRequiresOctopusVersionMessage,
                    OctopusBuildInformation.BuildInformationRequiresOctopusVersion);
            }
            var link = await repository.Link("BuildInformation").ConfigureAwait(false);

            var resource = new OctopusPackageVersionBuildInformationResource
            {
                PackageId = packageId,
                Version = version,
                OctopusBuildInformation = octopusMetadata
            };

            return await repository.Client.Post<OctopusPackageVersionBuildInformationResource, OctopusPackageVersionBuildInformationMappedResource>(link, resource, new { overwriteMode = overwriteMode }, cancellationToken).ConfigureAwait(false);
        }

        public Task<ResourceCollection<OctopusPackageVersionBuildInformationMappedResource>> ListBuilds(string packageId, int skip = 0, int take = 30)
            => ListBuilds(packageId, CancellationToken.None, skip, take);

        public async Task<ResourceCollection<OctopusPackageVersionBuildInformationMappedResource>> ListBuilds(string packageId, CancellationToken cancellationToken, int skip = 0, int take = 30)
        {
            return await repository.Client.List<OctopusPackageVersionBuildInformationMappedResource>(await repository.Link("BuildInformation").ConfigureAwait(false), new { packageId = packageId, take, skip }, cancellationToken).ConfigureAwait(false);
        }

        public Task<ResourceCollection<OctopusPackageVersionBuildInformationMappedResource>> LatestBuilds(int skip = 0, int take = 30)
            => LatestBuilds(CancellationToken.None, skip, take);

        public async Task<ResourceCollection<OctopusPackageVersionBuildInformationMappedResource>> LatestBuilds(CancellationToken cancellationToken, int skip = 0, int take = 30)
        {
            return await repository.Client.List<OctopusPackageVersionBuildInformationMappedResource>(await repository.Link("BuildInformation").ConfigureAwait(false), new { latest = true, take, skip }, cancellationToken).ConfigureAwait(false);
        }

        public Task Delete(OctopusPackageVersionBuildInformationMappedResource buildInformation)
            => Delete(buildInformation, CancellationToken.None);

        public async Task Delete(OctopusPackageVersionBuildInformationMappedResource buildInformation, CancellationToken cancellationToken)
        {
            await repository.Client.Delete(await repository.Link("BuildInformation").ConfigureAwait(false), new { id = buildInformation.Id }, null, cancellationToken).ConfigureAwait(false);
        }

        public Task DeleteBuilds(IReadOnlyList<OctopusPackageVersionBuildInformationMappedResource> builds)
            => DeleteBuilds(builds, CancellationToken.None);

        public async Task DeleteBuilds(IReadOnlyList<OctopusPackageVersionBuildInformationMappedResource> builds, CancellationToken cancellationToken)
        {
            await repository.Client.Delete(await repository.Link("BuildInformationBulk").ConfigureAwait(false), new { ids = builds.Select(p => p.Id).ToArray() }, null, cancellationToken).ConfigureAwait(false);
        }
    }

    public interface IBuildInformationRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<OctopusPackageVersionBuildInformationMappedResource> Get(string id);
        Task<OctopusPackageVersionBuildInformationMappedResource> Get(string id, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<OctopusPackageVersionBuildInformationMappedResource> Push(string packageId, string version, OctopusBuildInformation octopusMetadata, OverwriteMode overwriteMode);
        Task<OctopusPackageVersionBuildInformationMappedResource> Push(string packageId, string version, OctopusBuildInformation octopusMetadata, OverwriteMode overwriteMode, CancellationToken cancellationToken);
        [Obsolete]
        Task<OctopusPackageVersionBuildInformationMappedResource> Push(string packageId, string version, OctopusBuildInformation octopusMetadata, bool replaceExisting);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<OctopusPackageVersionBuildInformationMappedResource>> ListBuilds(string packageId, int skip = 0, int take = 30);
        Task<ResourceCollection<OctopusPackageVersionBuildInformationMappedResource>> ListBuilds(string packageId, CancellationToken cancellationToken, int skip = 0, int take = 30);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<OctopusPackageVersionBuildInformationMappedResource>> LatestBuilds(int skip = 0, int take = 30);
        Task<ResourceCollection<OctopusPackageVersionBuildInformationMappedResource>> LatestBuilds(CancellationToken cancellationToken, int skip = 0, int take = 30);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Delete(OctopusPackageVersionBuildInformationMappedResource buildInformation);
        Task Delete(OctopusPackageVersionBuildInformationMappedResource buildInformation, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task DeleteBuilds(IReadOnlyList<OctopusPackageVersionBuildInformationMappedResource> builds);
        Task DeleteBuilds(IReadOnlyList<OctopusPackageVersionBuildInformationMappedResource> builds, CancellationToken cancellationToken);
    }
}
