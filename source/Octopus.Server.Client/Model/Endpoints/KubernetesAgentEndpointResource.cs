using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints;

public class KubernetesAgentEndpointResource : EndpointResource
{
    public override CommunicationStyle CommunicationStyle => CommunicationStyle.KubernetesAgent;

    public TentacleEndpointConfiguration TentacleEndpointConfiguration { get; set; }

    public KubernetesConfiguration KubernetesConfiguration { get; set; }
}

public class KubernetesConfiguration
{
    public string DefaultJobExecutionContainer { get; set; }

    public string DefaultJobExecutionContainerFeed { get; set; }
}

public abstract class TentacleEndpointConfiguration : ITentacleEndpointConfiguration
{
    public abstract AgentCommunicationBehaviour CommunicationBehaviour { get; }

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

public sealed class PollingTentacleEndpointConfiguration : TentacleEndpointConfiguration, IPollingTentacleEndpointConfiguration
{
    public override AgentCommunicationBehaviour CommunicationBehaviour => AgentCommunicationBehaviour.Polling;
}

public sealed class ListeningTentacleEndpointConfiguration : TentacleEndpointConfiguration, IListeningTentacleEndpointConfiguration
{
    public override AgentCommunicationBehaviour CommunicationBehaviour => AgentCommunicationBehaviour.Listening;

    [Writeable]
    public string ProxyId { get; set; }
}