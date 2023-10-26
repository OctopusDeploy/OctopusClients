using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public abstract class TentacleEndpointResource : EndpointResource, ITentacleEndpointResource
    {
        [Required(ErrorMessage = "Please provide a thumbprint for this machine.")]
        [Trim]
        [Writeable]
        public string Thumbprint { get; set; }

        [Required(ErrorMessage = "Please provide a URI for this machine.")]
        [Trim]
        [Writeable]
        public string Uri { get; set; }

        [Writeable]
        public TentacleDetailsResource TentacleVersionDetails { get; set; }

        public string CertificateSignatureAlgorithm { get; set; }
    }
}