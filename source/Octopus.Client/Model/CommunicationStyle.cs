using System;
using Octopus.Client.Model.Accounts;

namespace Octopus.Client.Model
{
    public enum CommunicationStyle
    {
        None = 0,

        /// <summary>
        /// Listening
        /// </summary>
        [ScriptConsoleSupported] [TentacleUpgradeSupported] TentaclePassive = 1,

        /// <summary>
        /// Polling
        /// </summary>
        [ScriptConsoleSupported] [TentacleUpgradeSupported] TentacleActive = 2,

        [ScriptConsoleSupported] [SupportedAccountTypes(AccountType.SshKeyPair, AccountType.UsernamePassword)] Ssh = 3,

        OfflineDrop = 4,

        [ScriptConsoleSupported] AzureWebApp = 5,

        [SupportedAccountTypes(AccountType.UsernamePassword)] Ftp = 6,

        [SupportedAccountTypes(AccountType.AzureSubscription)] [ScriptConsoleSupported] AzureCloudService = 7,

        AzureServiceFabricCluster = 8,

        [SupportedAccountTypes(AccountType.UsernamePassword)] [ScriptConsoleSupported] Kubernetes = 9
    }
}