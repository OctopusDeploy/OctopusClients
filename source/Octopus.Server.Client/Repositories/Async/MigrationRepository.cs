using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.Migrations;

namespace Octopus.Client.Repositories.Async
{
    public interface IMigrationRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<MigrationPartialExportResource> PartialExport(MigrationPartialExportResource resource);
        Task<MigrationPartialExportResource> PartialExport(MigrationPartialExportResource resource, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<MigrationImportResource> Import(MigrationImportResource resource);
        Task<MigrationImportResource> Import(MigrationImportResource resource, CancellationToken cancellationToken);
    }

    class MigrationRepository : IMigrationRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public MigrationRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MigrationPartialExportResource> PartialExport(MigrationPartialExportResource resource)
            => PartialExport(resource, CancellationToken.None);

        public async Task<MigrationPartialExportResource> PartialExport(MigrationPartialExportResource resource, CancellationToken cancellationToken)
        {
            return await repository.Client.Post<MigrationPartialExportResource, MigrationPartialExportResource>(await repository.Link("MigrationsPartialExport").ConfigureAwait(false), resource, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MigrationImportResource> Import(MigrationImportResource resource)
            => Import(resource, CancellationToken.None);

        public async Task<MigrationImportResource> Import(MigrationImportResource resource, CancellationToken cancellationToken)
        {
            return await repository.Client.Post<MigrationImportResource, MigrationImportResource>(await repository.Link("MigrationsImport").ConfigureAwait(false), resource, cancellationToken).ConfigureAwait(false);
        }
    }
}
