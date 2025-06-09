using System.ComponentModel;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Authentication.OpenIDConnect.GoogleApps
{
    [Description("Sign in to your Octopus Server with Google Apps. [Learn more](https://g.octopushq.com/AuthGoogleApps).")]
    public class GoogleAppsConfigurationResource : OpenIDConnectConfigurationResource
    {
        public GoogleAppsConfigurationResource()
        {
            Id = "authentication-googleapps";
        }

        [DisplayName("Hosted Domain")]
        [Description("Tell Octopus which Google Apps domain to trust")]
        [Writeable]
        public string HostedDomain { get; set; }
    }
}
