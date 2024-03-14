namespace Octopus.Client.Model.Endpoints;

public class KubernetesTentacleEndpointResource : EndpointResource
{
    public override CommunicationStyle CommunicationStyle => CommunicationStyle.KubernetesTentacle;

    protected KubernetesTentacleEndpointResource()
    {
    }

    public KubernetesTentacleEndpointResource(TentacleEndpointConfigurationResource tentacleEndpointConfiguration)
    {
        TentacleEndpointConfiguration = tentacleEndpointConfiguration;
    }

    public TentacleEndpointConfigurationResource TentacleEndpointConfiguration { get; set; }

    public string DefaultNamespace { get; set; }
}