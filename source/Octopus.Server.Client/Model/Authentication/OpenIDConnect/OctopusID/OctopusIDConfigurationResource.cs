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
    }
}
