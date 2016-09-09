using System;

namespace Octopus.Client.Model
{
    public class CertificateResource : Resource, INamedResource
    {
        public string Name { get; set; }
        public string Thumbprint { get; set; }
    }
}