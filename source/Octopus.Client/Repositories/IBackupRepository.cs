using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IBackupRepository
    {
        BackupConfigurationResource GetConfiguration();
        BackupConfigurationResource ModifyConfiguration(BackupConfigurationResource backupConfiguration);
    }
}