namespace Octopus.Client.Model.Endpoints;

public class KubernetesTentacleEndpointResource : EndpointResource
{
    public override CommunicationStyle CommunicationStyle => CommunicationStyle.KubernetesTentacle;

    protected KubernetesTentacleEndpointResource()
    {
    }

    public KubernetesTentacleEndpointResource(KubernetesTentacleEndpointConfigurationResource kubernetesEndpointConfiguration)
    {
        KubernetesEndpointConfiguration = kubernetesEndpointConfiguration;
    }

    public KubernetesTentacleEndpointConfigurationResource KubernetesEndpointConfiguration { get; set; }

    public string DefaultNamespace { get; set; }
}