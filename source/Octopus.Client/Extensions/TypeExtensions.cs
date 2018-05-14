using System;
using System.Reflection;
using Octopus.Client.Model.Accounts;

namespace Octopus.Client.Extensions
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type t) => t.GetTypeInfo().IsValueType ? Activator.CreateInstance(t) : null;

        public static AccountType DetermineAccountType(this Type type)
        {
            var accountType = AccountType.None;

            if (type == typeof(UsernamePasswordAccountResource))
                accountType = AccountType.UsernamePassword;
            else if (type == typeof(SshKeyPairAccountResource))
                accountType = AccountType.SshKeyPair;
            else if (type == typeof(AzureServicePrincipalAccountResource))
                accountType = AccountType.AzureServicePrincipal;
            else if (type == typeof(AzureSubscriptionAccountResource))
                accountType = AccountType.AzureSubscription;
            else if (type == typeof(AmazonWebServicesAccountResource))
                accountType = AccountType.AmazonWebServicesAccount;
            else if (type == typeof(AmazonWebServicesRoleAccountResource))
                accountType = AccountType.AmazonWebServicesRoleAccount;
            else
                throw new ArgumentException($"Account type {type} is not supported");
            return accountType;
        }
    }
}