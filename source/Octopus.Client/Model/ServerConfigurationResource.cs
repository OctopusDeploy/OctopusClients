using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class ServerConfigurationResource : Resource
    {
        [Writeable]
        [Trim]
        public string ServerUri { get; set; }
    }
}