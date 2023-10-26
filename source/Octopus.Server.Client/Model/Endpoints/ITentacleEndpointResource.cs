namespace Octopus.Client.Model.Endpoints
{
    public interface ITentacleEndpointResource
    {
        string Thumbprint { get; set; }

        string Uri { get; set; }

        TentacleDetailsResource TentacleVersionDetails { get; set; }

        string CertificateSignatureAlgorithm { get; set; }
    }
}