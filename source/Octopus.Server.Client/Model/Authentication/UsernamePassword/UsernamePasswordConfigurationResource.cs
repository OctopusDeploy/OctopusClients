using Octopus.Client.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Client.Model.Authentication.UsernamePassword
{
    public class UsernamePasswordConfigurationResource : ExtensionConfigurationResource
    {
        public UsernamePasswordConfigurationResource()
        {
            Id = "authentication-usernamepassword";
        }
    }
}
