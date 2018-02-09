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
        readonly IOctopusAsyncClient client;

        public MigrationRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public async Task<MigrationPartialExportResource> PartialExport(MigrationPartialExportResource resource)
        {
            return await client.Post<MigrationPartialExportResource, MigrationPartialExportResource>(client.RootDocument.Link("MigrationsPartialExport"), resource).ConfigureAwait(false);
        }

        public async Task<MigrationImportResource> Import(MigrationImportResource resource)
        {
            return await client.Post<MigrationImportResource, MigrationImportResource>(client.RootDocument.Link("MigrationsImport"), resource).ConfigureAwait(false);
        }
    }
}