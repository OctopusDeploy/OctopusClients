namespace Octopus.Client.Model
{
    public class CertificateConfigurationResource : Resource, INamedResource
    {
        public string Name { get; set; }
        public string Thumbprint { get; set; }
    }
}