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
            return await repository.Client.Post<MigrationPartialExportResource, MigrationPartialExportResource>(repository.Link("MigrationsPartialExport"), resource).ConfigureAwait(false);
        }

        public async Task<MigrationImportResource> Import(MigrationImportResource resource)
        {
            return await repository.Client.Post<MigrationImportResource, MigrationImportResource>(repository.Link("MigrationsImport"), resource).ConfigureAwait(false);
        }
    }
}