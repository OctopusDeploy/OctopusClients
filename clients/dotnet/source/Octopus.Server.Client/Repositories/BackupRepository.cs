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
        private readonly IOctopusRepository repository;

        public BackupRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public BackupConfigurationResource GetConfiguration()
        {
            return repository.Client.Get<BackupConfigurationResource>(repository.Link("BackupConfiguration"));
        }

        public BackupConfigurationResource ModifyConfiguration(BackupConfigurationResource backupConfiguration)
        {
            return repository.Client.Update(backupConfiguration.Link("Self"), backupConfiguration);
        }
    }
}