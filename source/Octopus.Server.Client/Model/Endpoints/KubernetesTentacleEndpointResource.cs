namespace Octopus.Client.Model.Endpoints;

public class KubernetesTentacleEndpointResource : EndpointResource
{
    public override CommunicationStyle CommunicationStyle => CommunicationStyle.KubernetesTentacle;

    protected KubernetesTentacleEndpointResource()
    {
    }

    public KubernetesTentacleEndpointResource(KubernetesTentacleEndpointConfigurationResource endpointConfiguration)
    {
        EndpointConfiguration = endpointConfiguration;
    }

    public KubernetesTentacleEndpointConfigurationResource EndpointConfiguration { get; set; }

    public string DefaultNamespace { get; set; }
}