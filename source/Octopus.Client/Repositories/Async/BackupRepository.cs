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
        readonly IOctopusAsyncClient client;

        public BackupRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public Task<BackupConfigurationResource> GetConfiguration()
        {
            return client.Get<BackupConfigurationResource>(client.RootDocument.Link("BackupConfiguration"));
        }

        public Task<BackupConfigurationResource> ModifyConfiguration(BackupConfigurationResource backupConfiguration)
        {
            return client.Update(backupConfiguration.Link("Self"), backupConfiguration);
        }
    }
}
