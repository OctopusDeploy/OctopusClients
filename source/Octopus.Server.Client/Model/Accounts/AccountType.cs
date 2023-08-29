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
        AmazonWebServicesAccount,
        AmazonWebServicesRoleAccount,
        Token,
        GoogleCloudAccount,
        AzureOidcAccount,
    }
}