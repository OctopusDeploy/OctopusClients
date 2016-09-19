using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IBackupRepository
    {
        Task<BackupConfigurationResource> GetConfiguration();
        Task<BackupConfigurationResource> ModifyConfiguration(BackupConfigurationResource backupConfiguration);
    }
}