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
        private readonly IOctopusRepository repository;

        public MigrationRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public MigrationPartialExportResource PartialExport(MigrationPartialExportResource resource)
        {
            return repository.Client.Post<MigrationPartialExportResource, MigrationPartialExportResource>(repository.Link("MigrationsPartialExport"), resource);
        }

        public MigrationImportResource Import(MigrationImportResource resource)
        {
            return repository.Client.Post< MigrationImportResource, MigrationImportResource>(repository.Link("MigrationsImport"), resource);
        }
    }
}