using Octopus.Client.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Client.Model.Authentication.Guest
{
    public class GuestConfigurationResource : ExtensionConfigurationResource
    {
        public GuestConfigurationResource()
        {
            Id = "authentication-guest";
        }
    }
}
