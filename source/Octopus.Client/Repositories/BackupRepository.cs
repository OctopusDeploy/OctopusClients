using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IBackupRepository
    {
        BackupConfigurationResource GetConfiguration();
        BackupConfigurationResource ModifyConfiguration(BackupConfigurationResource backupConfiguration);
    }
    
    class BackupRepository : IBackupRepository
    {
        readonly IOctopusClient client;

        public BackupRepository(IOctopusClient client)
        {
            this.client = client;
        }

        public BackupConfigurationResource GetConfiguration()
        {
            return client.Get<BackupConfigurationResource>(client.RootDocument.Link("BackupConfiguration"));
        }

        public BackupConfigurationResource ModifyConfiguration(BackupConfigurationResource backupConfiguration)
        {
            return client.Update(backupConfiguration.Link("Self"), backupConfiguration);
        }
    }
}