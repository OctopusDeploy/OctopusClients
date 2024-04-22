namespace Octopus.Client.Model.Endpoints
{
    public interface ITentacleEndpointResource
    {
        string Thumbprint { get; set; }
        string Uri { get; set; }
        string CertificateSignatureAlgorithm { get; set; }
    }
}