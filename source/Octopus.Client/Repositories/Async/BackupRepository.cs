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

        public Task<BackupConfigurationResource> GetConfiguration()
        {
            return client.Get<BackupConfigurationResource>(repository.Link("BackupConfiguration"));
        }

        public Task<BackupConfigurationResource> ModifyConfiguration(BackupConfigurationResource backupConfiguration)
        {
            return client.Update(backupConfiguration.Link("Self"), backupConfiguration);
        }
    }
}
