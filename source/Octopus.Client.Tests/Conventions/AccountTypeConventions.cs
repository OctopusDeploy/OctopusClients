using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Octopus.Client.Extensions;
using Octopus.Client.Model.Accounts;

namespace Octopus.Client.Tests.Conventions
{
    [TestFixture]
    public class AccountTypeConventions
    {
        private static readonly TypeInfo[] ExportedTypes = typeof(AccountResource).GetTypeInfo().Assembly.GetExportedTypes().Select(t => t.GetTypeInfo()).ToArray();

        [Test]
        public void AllAccountResourceTypeCanBeMappedToAnAccountType()
        {
            var derivedAccountTypes = ExportedTypes
                .Where(t => !t.IsAbstract)
                .Where(t => typeof(AccountResource).IsAssignableFrom(t))
                .ToArray();

            var typesThatCannotBeMapped = derivedAccountTypes.Where(t =>
                {
                    try
                    {
                        t.DetermineAccountType();
                        return false;
                    }
                    catch (ArgumentException)
                    {
                        return true;
                    }
                })
                .ToArray();

            if (typesThatCannotBeMapped.Any())
                Assert.Fail($"The following AccountResource types cannot be mapped to an AccountType: " + typesThatCannotBeMapped.CommaSeperate());
        }
    }
}