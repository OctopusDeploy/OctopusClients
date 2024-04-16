using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public abstract class KubernetesTentacleEndpointConfigurationResource : ITentacleEndpointResource
    {
        protected KubernetesTentacleEndpointConfigurationResource()
        {
        }

        protected KubernetesTentacleEndpointConfigurationResource(string thumbprint, string uri)
        {
            Thumbprint = thumbprint;
            Uri = uri;
        }


        public abstract TentacleCommunicationModeResource CommunicationMode { get; }

        [Required(ErrorMessage = "Please provide a thumbprint for this machine.")]
        [Trim]
        [Writeable]
        public string Thumbprint { get; set; }

        [Trim]
        [Writeable]
        public string Uri { get; set; }

        [Writeable]
        public TentacleDetailsResource TentacleVersionDetails { get; set; }

        public string CertificateSignatureAlgorithm { get; set; }
    }
}