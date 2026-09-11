using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IBackupRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<BackupConfigurationResource> GetConfiguration();
        Task<BackupConfigurationResource> GetConfiguration(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<BackupConfigurationResource> ModifyConfiguration(BackupConfigurationResource backupConfiguration);
        Task<BackupConfigurationResource> ModifyConfiguration(BackupConfigurationResource backupConfiguration, CancellationToken cancellationToken);
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
            => GetConfiguration(CancellationToken.None);

        public async Task<BackupConfigurationResource> GetConfiguration(CancellationToken cancellationToken)
        {
            return await client.Get<BackupConfigurationResource>(await repository.Link("BackupConfiguration").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        public Task<BackupConfigurationResource> ModifyConfiguration(BackupConfigurationResource backupConfiguration)
            => ModifyConfiguration(backupConfiguration, CancellationToken.None);

        public Task<BackupConfigurationResource> ModifyConfiguration(BackupConfigurationResource backupConfiguration, CancellationToken cancellationToken)
        {
            return client.Update(backupConfiguration.Link("Self"), backupConfiguration, cancellationToken);
        }
    }
}
