namespace Octopus.Client.Model.Endpoints
{
    public interface IListeningTentacleEndpointResource : ITentacleEndpointResource
    {
        string ProxyId { get; set; }
    }
}