using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IBackupRepository
    {
        Task<BackupConfigurationResource> GetConfiguration();
        Task<BackupConfigurationResource> ModifyConfiguration(BackupConfigurationResource backupConfiguration);
    }

    class BackupRepository : IBackupRepository
    {
        private readonly IOctopusAsyncRepository repository;
        readonly IOctopusAsyncClient client;

        public BackupRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
            this.client = repository.Client;
        }

        public async Task<BackupConfigurationResource> GetConfiguration()
        {
            return await client.Get<BackupConfigurationResource>(await repository.Link("BackupConfiguration").ConfigureAwait(false));
        }

        public Task<BackupConfigurationResource> ModifyConfiguration(BackupConfigurationResource backupConfiguration)
        {
            return client.Update(backupConfiguration.Link("Self"), backupConfiguration);
        }
    }
}
