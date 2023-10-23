using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public interface ITentacleEndpointConfiguration
    {
        string Thumbprint { get; set; }
        TentacleDetailsResource TentacleVersionDetails { get; set; }
        string CertificateSignatureAlgorithm { get; set; }
        string Uri { get; set; }
    }

    public abstract class TentacleEndpointResource : EndpointResource, ITentacleEndpointConfiguration
    {
        [Required(ErrorMessage = "Please provide a thumbprint for this machine.")]
        [Trim]
        [Writeable]
        public string Thumbprint { get; set; }

        [Writeable]
        public TentacleDetailsResource TentacleVersionDetails { get; set; }

        public string CertificateSignatureAlgorithm { get; set; }

        [Trim]
        [Writeable]
        public string Uri { get; set; }
    }
}