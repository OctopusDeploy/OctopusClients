namespace Octopus.Client.Model.Endpoints;

public class KubernetesAgentEndpointResource : EndpointResource
{
    public override CommunicationStyle CommunicationStyle => CommunicationStyle.KubernetesAgent;

    protected KubernetesAgentEndpointResource()
    {
    }

    public KubernetesAgentEndpointResource(TentacleEndpointConfigurationResource tentacleEndpointConfiguration, DeploymentActionContainerResource defaultJobExecutionContainer)
    {
        TentacleEndpointConfiguration = tentacleEndpointConfiguration;
        DefaultJobExecutionContainer = defaultJobExecutionContainer;
    }

    public TentacleEndpointConfigurationResource TentacleEndpointConfiguration { get; set; }

    public DeploymentActionContainerResource DefaultJobExecutionContainer { get; set; }
}