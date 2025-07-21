using System;

namespace Octopus.Client.Model.Accounts
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SupportedAccountTypesAttribute : Attribute
    {
        readonly AccountType[] accountTypes;

        public SupportedAccountTypesAttribute(params AccountType[] accountTypes)
        {
            this.accountTypes = accountTypes;
        }

        public AccountType[] AccountTypes
        {
            get { return accountTypes; }
        }
    }
}