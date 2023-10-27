using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public abstract class TentacleEndpointConfigurationResource : ITentacleEndpointResource
    {
        protected TentacleEndpointConfigurationResource()
        {
        }

        protected TentacleEndpointConfigurationResource(string uri, string thumbprint)
        {
            Uri = uri;
            Thumbprint = thumbprint;
        }


        public abstract AgentCommunicationStyleResource CommunicationStyleResource { get; }

        [Trim]
        [Writeable]
        public string Uri { get; set; }

        [Required(ErrorMessage = "Please provide a thumbprint for this machine.")]
        [Trim]
        [Writeable]
        public string Thumbprint { get; set; }

        [Writeable]
        public TentacleDetailsResource TentacleVersionDetails { get; set; }

        public string CertificateSignatureAlgorithm { get; set; }
    }
}