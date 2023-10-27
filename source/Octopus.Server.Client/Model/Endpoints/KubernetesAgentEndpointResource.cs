namespace Octopus.Client.Model.Endpoints;

public class KubernetesAgentEndpointResource : EndpointResource
{
    public override CommunicationStyle CommunicationStyle => CommunicationStyle.KubernetesAgent;

    protected KubernetesAgentEndpointResource()
    {
    }

    public KubernetesAgentEndpointResource(TentacleEndpointConfigurationResource tentacleEndpointConfiguration)
    {
        TentacleEndpointConfiguration = tentacleEndpointConfiguration;
    }

    public TentacleEndpointConfigurationResource TentacleEndpointConfiguration { get; set; }
}