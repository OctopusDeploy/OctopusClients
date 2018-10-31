using System.Threading.Tasks;
using Octopus.Client.Model.Migrations;

namespace Octopus.Client.Repositories.Async
{
    public interface IMigrationRepository
    {
        Task<MigrationPartialExportResource> PartialExport(MigrationPartialExportResource resource);
        Task<MigrationImportResource> Import(MigrationImportResource resource);
    }

    class MigrationRepository : IMigrationRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public MigrationRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public async Task<MigrationPartialExportResource> PartialExport(MigrationPartialExportResource resource)
        {
            return await repository.Client.Post<MigrationPartialExportResource, MigrationPartialExportResource>(await repository.Link("MigrationsPartialExport").ConfigureAwait(false), resource).ConfigureAwait(false);
        }

        public async Task<MigrationImportResource> Import(MigrationImportResource resource)
        {
            return await repository.Client.Post<MigrationImportResource, MigrationImportResource>(await repository.Link("MigrationsImport").ConfigureAwait(false), resource).ConfigureAwait(false);
        }
    }
}