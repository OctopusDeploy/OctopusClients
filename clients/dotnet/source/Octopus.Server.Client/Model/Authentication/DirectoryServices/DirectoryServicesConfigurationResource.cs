#nullable enable
using System.Net;
using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Client.Model.Authentication.DirectoryServices
{
    public class DirectoryServicesConfigurationResource : ExtensionConfigurationResource
    {
        public DirectoryServicesConfigurationResource()
        {
            Id = "authentication-directoryservices";
        }

        [Writeable]
        public string? ActiveDirectoryContainer { get; set; }

        [Writeable]
        public AuthenticationSchemes AuthenticationScheme { get; set; }

        [Writeable]
        public bool AllowFormsAuthenticationForDomainUsers { get; set; }

        [Writeable]
        public bool AreSecurityGroupsEnabled { get; set; }

        [Writeable]
        public bool? AllowAutoUserCreation { get; set; }
    }
}
