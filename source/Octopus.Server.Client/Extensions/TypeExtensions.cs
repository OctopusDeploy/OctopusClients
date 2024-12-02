using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Octopus.Client.Model.Accounts;

namespace Octopus.Client.Extensions
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type t) =>
            t.GetTypeInfo().IsValueType ? Activator.CreateInstance(t) : null;

        public static AccountType DetermineAccountType(this Type type)
        {
            AccountType accountType;
            if (type == typeof(UsernamePasswordAccountResource))
                accountType = AccountType.UsernamePassword;
            else if (type == typeof(SshKeyPairAccountResource))
                accountType = AccountType.SshKeyPair;
            else if (type == typeof(AzureServicePrincipalAccountResource))
                accountType = AccountType.AzureServicePrincipal;
            else if (type == typeof(AzureSubscriptionAccountResource))
                accountType = AccountType.AzureSubscription;
            else if (type == typeof(AzureOidcAccountResource))
                accountType = AccountType.AzureOidc;
            else if (type == typeof(AmazonWebServicesAccountResource))
                accountType = AccountType.AmazonWebServicesAccount;
            else if (type == typeof(TokenAccountResource))
                accountType = AccountType.Token;
            else if (type == typeof(GoogleCloudAccountResource))
                accountType = AccountType.GoogleCloudAccount;
            else if (type == typeof(GenericOidcAccountResource))
                accountType = AccountType.GenericOidcAccount;
            else
                throw new ArgumentException($"Account type {type} is not supported");

            return accountType;
        }

        internal static bool IsClosedTypeOfOpenGeneric(this Type type, Type openGenericType)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (!openGenericType.IsGenericType)
                throw new ArgumentException("Must be a generic type", nameof(openGenericType));
            if (openGenericType.IsConstructedGenericType)
                throw new ArgumentException("Must be an open generic type", nameof(openGenericType));

            var completeTypeHierarchy = type.CompleteTypeHierarchy();

            var constructedGenericTypesInHierarchy = completeTypeHierarchy
                .Where(t => t.IsConstructedGenericType)
                .ToArray();

            var openGenericTypesInHierarchy = constructedGenericTypesInHierarchy
                .Select(t => t.GetGenericTypeDefinition())
                .ToArray();

            var result = openGenericTypesInHierarchy.Contains(openGenericType);
            return result;
        }

        internal static IEnumerable<Type> CompleteTypeHierarchy(this Type type)
        {
            var hierarchy = new[] {type}
                .DepthFirst(t => t.GetInterfaces()
                    .Union(new[] {t.BaseType})
                    .Where(u => !(u is null))
                )
                .Distinct()
                .ToArray();

            return hierarchy;
        }

        internal static IEnumerable<T> DepthFirst<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childrenFunc)
        {
            foreach (var item in items)
            {
                yield return item;

                foreach (var descendant in childrenFunc(item).DepthFirst(childrenFunc)) yield return descendant;
            }
        }
    }
}