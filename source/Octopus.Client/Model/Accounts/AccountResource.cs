using System;

namespace Octopus.Client.Model.Accounts
{
    public abstract class AccountResource : Resource, INamedResource
    {
        protected AccountResource()
        {
            EnvironmentIds = new ReferenceCollection();
            TenantTags = new ReferenceCollection();
            TenantIds = new ReferenceCollection();
        }

        [Writeable]
        [Trim]
        public string Name { get; set; }

        [Writeable]
        public string Description { get; set; }

        [Writeable]
        public ReferenceCollection EnvironmentIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantTags { get; set; }

        [WriteableOnCreate]
        public abstract AccountType AccountType { get; }
    }
}