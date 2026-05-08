#nullable enable
using System;
using System.ComponentModel;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Authentication.OpenIDConnect.OctopusID
{
    [Description("Sign in to your Octopus Server with your Octopus ID. [Learn more](https://g.octopushq.com/AuthOctopusID).")]
    public class OctopusIDConfigurationResource : OpenIDConnectConfigurationResource
    {
        public OctopusIDConfigurationResource()
        {
            Id = "authentication-octopusid";
        }

        [DisplayName("Allow Dynamic Registration")]
        [Description("Allow Octopus Server to register itself automatically as a client for Octopus ID")]
        [Writeable]
        public bool? AllowDynamicRegistration { get; set; }

        [DisplayName("Allow M2M Dynamic Registration")]
        [Description("Allow Octopus Server to register itself automatically as an M2M client for Octopus ID")]
        [Writeable]
        public bool? AllowM2MDynamicRegistration { get; set; }

        [DisplayName("M2M Client ID")]
        [Description("The client ID issued by Octopus ID via the M2M dynamic-registration flow.")]
        [Writeable]
        public string? M2MClientId { get; set; }

        [DisplayName("M2M Resource Server Identifier")]
        [Description("The resource server identifier issued by Octopus ID via the M2M dynamic-registration flow.")]
        [Writeable]
        public string? M2MResourceServerIdentifier { get; set; }

        [DisplayName("M2M Registered At")]
        [Description("The timestamp at which Octopus Server was registered as an M2M client with Octopus ID.")]
        [Writeable]
        public DateTimeOffset? M2MRegisteredAt { get; set; }
    }
}
