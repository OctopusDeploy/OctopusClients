using Octopus.Client.Model.Migrations;

namespace Octopus.Client.Repositories
{
    public interface IMigrationRepository
    {
        MigrationPartialExportResource PartialExport(MigrationPartialExportResource resource);
        MigrationImportResource Import(MigrationImportResource resource);
    }

    class MigrationRepository : IMigrationRepository
    {
        readonly IOctopusClient client;

        public MigrationRepository(IOctopusClient client)
        {
            this.client = client;
        }

        public MigrationPartialExportResource PartialExport(MigrationPartialExportResource resource)
        {
            return client.Post<MigrationPartialExportResource, MigrationPartialExportResource>(client.Link("MigrationsPartialExport"), resource);
        }

        public MigrationImportResource Import(MigrationImportResource resource)
        {
            return client.Post< MigrationImportResource, MigrationImportResource>(client.Link("MigrationsImport"), resource);
        }
    }
}