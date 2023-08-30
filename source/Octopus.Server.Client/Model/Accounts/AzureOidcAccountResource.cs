﻿using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Accounts
{
    public class AzureOidcAccountResource : AccountResource
    {
        public override AccountType AccountType => AccountType.AzureOidcAccount;

        [Trim]
        [Writeable]
        [Required(ErrorMessage = "Please provide an Azure subscription ID.")]
        public string SubscriptionNumber { get; set; }

        [Trim]
        [Writeable]
        [NotDocumentReference]
        public string ClientId { get; set; }

        [Trim]
        [Writeable]
        [NotDocumentReference]
        public string TenantId { get; set; }

        [Trim]
        [Writeable]
        public string AzureEnvironment { get; set; }

        [Writeable]
        public bool TenantedSubjectGeneration { get; set; }

        [Trim]
        [Writeable]
        public string ResourceManagementEndpointBaseUri { get; set; }

        [Trim]
        [Writeable]
        public string ActiveDirectoryEndpointBaseUri { get; set; }
        
        public class WebSite
        {
            public string Name { get; set; }
            public string WebSpace { get; set; }
            public string ResourceGroup { get; set; }
        }

        public class WebSlot
        {
            public string Name { get; set; }
            public string Site { get; set; }
            public string ResourceGroupName { get; set; }
            public string Region { get; set; }
        }

        public class ResourceGroup
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}