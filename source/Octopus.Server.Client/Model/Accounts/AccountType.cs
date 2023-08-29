using System;

namespace Octopus.Client.Model.Accounts
{
    public enum AccountType
    {
        None,
        UsernamePassword,
        SshKeyPair,
        AzureSubscription,
        AzureServicePrincipal,
        AzureOidcAccount,
        AmazonWebServicesAccount,
        AmazonWebServicesRoleAccount,
        Token,
        GoogleCloudAccount,
    }
}