using System;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class ProxyResource : Resource, INamedResource, IHaveSpaceResource
    {
        public ProxyResource()
        {
            Password = new SensitiveValue();
        }

        [Writeable]
        [Trim]
        public string Name { get; set; }

        [Writeable]
        [Trim]
        public string Host { get; set; }

        [Writeable]
        public int Port { get; set; }

        [Writeable]
        public string ProxyType { get; set; }

        [Writeable]
        public string Username { get; set; }

        [Writeable]
        public SensitiveValue Password { get; set; }

        public string SpaceId { get; set; }
    }
}