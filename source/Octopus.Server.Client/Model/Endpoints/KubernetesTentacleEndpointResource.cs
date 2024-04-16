namespace Octopus.Client.Model.Endpoints;

public class KubernetesTentacleEndpointResource : EndpointResource
{
    public override CommunicationStyle CommunicationStyle => CommunicationStyle.KubernetesTentacle;

    protected KubernetesTentacleEndpointResource()
    {
    }

    public KubernetesTentacleEndpointResource(KubernetesTentacleEndpointConfigurationResource tentacleEndpointConfiguration)
    {
        TentacleEndpointConfiguration = tentacleEndpointConfiguration;
    }

    public KubernetesTentacleEndpointConfigurationResource TentacleEndpointConfiguration { get; set; }

    public KubernetesAgentDetailsResource KubernetesAgentDetails { get; set; }

    public KubernetesAgentUpgradeInformationResource UpgradeInformation { get; set; }

    public string DefaultNamespace { get; set; }
}