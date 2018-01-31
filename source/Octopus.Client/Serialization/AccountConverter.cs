using System;
using System.Collections.Generic;
using Octopus.Client.Model;
using Octopus.Client.Model.Accounts;

namespace Octopus.Client.Serialization
{
    public class AccountConverter : InheritedClassConverter<AccountResource, AccountType>
    {
        static readonly IDictionary<AccountType, Type> AccountTypeMappings =
            new Dictionary<AccountType, Type>
            {
                {AccountType.UsernamePassword, typeof(UsernamePasswordAccountResource)},
                {AccountType.AzureSubscription, typeof(AzureSubscriptionAccountResource)},
                {AccountType.AzureServicePrincipal, typeof(AzureServicePrincipalAccountResource)},
                {AccountType.SshKeyPair, typeof(SshKeyPairAccountResource)},
                {AccountType.AmazonWebServicesAccount, typeof(AmazonWebServicesAccountResource)},
                {AccountType.AmazonWebServicesRoleAccount, typeof(AmazonWebServicesRoleAccountResource)}
            };

        protected override IDictionary<AccountType, Type> DerivedTypeMappings => AccountTypeMappings;
        protected override string TypeDesignatingPropertyName => "AccountType";
    }
}