using System.ComponentModel;

namespace Octopus.Client.Model.Authentication.OpenIDConnect.OctopusID
{
    [Description("Sign in to your Octopus Server with your Octopus ID. [Learn more](https://g.octopushq.com/AuthOctopusID).")]
    public class OctopusIDConfigurationResource : OpenIDConnectConfigurationResource
    {
        public OctopusIDConfigurationResource()
        {
            Id = "authentication-octopusid";
        }
    }
}
